using Ecom.Application.DTOs.Authentication;
using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly IJwtService _jwtService;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IRefreshTokenService _refreshTokenService;
		private readonly IConfiguration _configuration;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailService _emailService;
		private readonly ILogger<AuthService> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService
			, IRefreshTokenRepository refreshTokenRepository, IRefreshTokenService refreshTokenService, IConfiguration configuration, IUnitOfWork unitOfWork, IEmailService emailService,
			ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_jwtService = jwtService;
			_roleManager = roleManager;
			_refreshTokenRepository = refreshTokenRepository;
			_refreshTokenService = refreshTokenService;
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_emailService = emailService;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}
	
		public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto)
		{
			_logger.LogInformation("Registration attempt started for Email: {Email}", dto.Email);
			if (await IsEmailAlreadyRegistered(dto.Email!))
			{
				_logger.LogWarning("Registration failed. Email already registered: {Email}", dto.Email);
				return new RegisterResponseDto() { IsSuccess=false , Message=  "Email Is Already Registered" };
			}  

			ApplicationUser user = new ApplicationUser() { FullName=dto.FullName! , PhoneNumber=dto.PhoneNumber,
				Email= dto.Email, UserName= dto.Email, CreatedAt= DateTime.UtcNow};

			var result =await _userManager.CreateAsync(user, dto.Password!);	 
			if (!result.Succeeded)
			{
				_logger.LogError("User creation failed for Email: {Email}. Errors: {Errors}",dto.Email,	string.Join(", ", result.Errors.Select(e => e.Description)));
				return new RegisterResponseDto() { IsSuccess = false, Message= "error while creating user" };
			}
			_logger.LogInformation("User created successfully. UserId: {UserId}, Email: {Email}",user.Id,user.Email);
			await _userManager.AddToRoleAsync(user, "User");
			await SendEmailConfirmationAsync(user.Email!);
			return new RegisterResponseDto() { IsSuccess = true, Message = "Registration successful. Please check your email to confirm your account." };
		}
		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
			var user = await _userManager.FindByEmailAsync(dto.Email!);
			if (user==null)
			{
				_logger.LogWarning("Login failed for non-existing email {Email} from IP {IP}", dto.Email, ip);
				return new AuthResponseDto() { IsSuccess=false, Errors= new List<string>() { "Email Or Password is Invalid" } };
			}
			if (await _userManager.IsLockedOutAsync(user)==true) //check if user is Locked
			{
				_logger.LogWarning("Locked account login attempt for UserId {UserId} from IP {IP}", user.Id, ip);
				return new AuthResponseDto() { IsSuccess = false, Errors = new List<string>() { "Your account is locked, Try again later"} };
			}
			var passwordcheck = await _userManager.CheckPasswordAsync(user, dto.Password!);
			if (!passwordcheck)
			{
				_logger.LogWarning("Invalid password attempt for UserId {UserId} from IP {IP}", user.Id, ip);
				await _userManager.AccessFailedAsync(user); //increment 1 attempt failed to the AccessFailedCount Property
				return new AuthResponseDto() { IsSuccess = false, Errors = new List<string>() { "Email Or Password is Invalid" } };
			}
			await _userManager.ResetAccessFailedCountAsync(user); //reset counter because user logged in successfully (password is right)

			if (user.EmailConfirmed==false)
			{
				_logger.LogInformation("Login attempt before email confirmation for UserId {UserId} from IP {IP}", user.Id, ip);
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Please confirm your email first." }
				};
			}
			var roles = await _userManager.GetRolesAsync(user);

			JwtUserDataDto jwtUserData = new JwtUserDataDto()
			{
				UserId = user.Id,
				FullName = user.FullName
				,
				Email = dto.Email!,
				Roles = roles
			};
			JwtResultDto TokenDto = await _jwtService.GenerateTokenAsync(jwtUserData);

			var session = await IssueNewSessionAsync(user.Id);
			_logger.LogInformation("User {UserId} logged in successfully from IP {IP}", user.Id, ip);
			return new AuthResponseDto() {IsSuccess=true, Token=TokenDto.Token, RefreshToken= session.RawToken, ExpiresAt= TokenDto.ExpiresAt };

		}
		private async Task<bool> IsEmailAlreadyRegistered(string email)
		{
			var result =await _userManager.FindByEmailAsync(email);
			if (result !=null)
			{
				return true;
			}
			return false;
		}
		private async Task<RefreshTokenResultDto> IssueNewSessionAsync(Guid userId)
		{
			_logger.LogInformation("Issuing new session for UserId: {UserId}", userId);
			int lifetimeDays = _configuration.GetValue<int>("RefreshToken:RefreshTokenLifetimeDays");
			DateTime absoluteExpirationTime = DateTime.UtcNow.AddDays(lifetimeDays);

			RefreshTokenResultDto refreshTokenresult =  _refreshTokenService.GenerateRefreshTokenAsync(absoluteExpirationTime);

			RefreshToken refreshEntity = new RefreshToken(userId, refreshTokenresult.HashedToken, refreshTokenresult.ExpiresAt);
			await _refreshTokenRepository.AddRefreshTokenAsync(refreshEntity);
			await _unitOfWork.SaveChangesAsync();
			_logger.LogInformation("New refresh token issued for UserId: {UserId}", userId);
			return refreshTokenresult;
		}
		public async Task<AuthResponseDto> RefreshSessionAsync(string refreshToken)
		{
			_logger.LogInformation("Refresh session attempt started");
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				_logger.LogWarning("Refresh failed: empty refresh token provided");
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}

			string hashedtoken=  _refreshTokenService.HashToken(refreshToken);

			#region RefreshToken Validations
			var refreshtokenfrom_db = await _refreshTokenRepository.GetByHashedTokenAsync(hashedtoken);
			if (refreshtokenfrom_db == null)
			{
				_logger.LogWarning("Refresh failed: token not found");
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}
			if (refreshtokenfrom_db.IsRevoked == true)
			{
				_logger.LogWarning("Refresh failed: revoked token detected for UserId: {UserId}", refreshtokenfrom_db.UserId);
				var user_tokens = await _refreshTokenRepository.GetAllUserTokensAsync(refreshtokenfrom_db.UserId);
				foreach (var token in user_tokens)
				{
					token.Revoke();
				}
				await _unitOfWork.SaveChangesAsync();

				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}
			if (refreshtokenfrom_db.IsExpired == true)
			{
				_logger.LogWarning("Refresh failed: expired token for UserId: {UserId}", refreshtokenfrom_db.UserId);
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Session expired. Please login again." }
				};
			} 
			#endregion

			#region Generate JWT access token
			ApplicationUser? user = await _userManager.FindByIdAsync(refreshtokenfrom_db.UserId.ToString());
			if (user == null)
			{
				_logger.LogError("Refresh failed: user not found for token");
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}
			var roles = await _userManager.GetRolesAsync(user);
			JwtUserDataDto jwtUserDataDto = new JwtUserDataDto() { UserId = user.Id, Email = user.Email!, FullName = user.FullName, Roles = roles };
			JwtResultDto jwtresult = await _jwtService.GenerateTokenAsync(jwtUserDataDto);
			_logger.LogInformation("New access token generated for UserId: {UserId}", user.Id);
			#endregion

			DateTime absoluteExpirationTime = refreshtokenfrom_db.ExpiresAt; //keep same absolute expiration

			RefreshTokenResultDto refreshTokenresult =  _refreshTokenService.GenerateRefreshTokenAsync(absoluteExpirationTime);
			refreshtokenfrom_db.Revoke();
			RefreshToken refreshEntity = new RefreshToken(user.Id, refreshTokenresult.HashedToken, refreshTokenresult.ExpiresAt);
			await _refreshTokenRepository.AddRefreshTokenAsync(refreshEntity);
			await _unitOfWork.SaveChangesAsync();
			_logger.LogInformation("Refresh session completed successfully for UserId: {UserId}", user.Id);

			return new AuthResponseDto() { IsSuccess = true, Token = jwtresult.Token, RefreshToken = refreshTokenresult.RawToken, ExpiresAt = jwtresult.ExpiresAt };
		}

		public async Task<bool> LogoutDeviceAsync(string refreshToken)
		{
			_logger.LogInformation("Logout device attempt started");
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				_logger.LogWarning("Logout failed: empty token");
				return false;
			}
			string HashedToken =_refreshTokenService.HashToken(refreshToken);
			RefreshToken? tokenfromdb = await _refreshTokenRepository.GetByHashedTokenAsync(HashedToken);

			if (tokenfromdb==null)
			{
				_logger.LogWarning("Logout failed: token not found");
				return false;
			}
			tokenfromdb.Revoke();
			await _unitOfWork.SaveChangesAsync();
			_logger.LogInformation("Device logged out successfully for UserId: {UserId}", tokenfromdb.UserId);
			return true;
		}

		public async Task<bool> LogoutAllDevicesAsync(Guid userId)
		{
			_logger.LogInformation("Logout all devices started for UserId: {UserId}", userId);

			if (userId==Guid.Empty)
			{
				_logger.LogWarning("Logout all failed: empty userId");
				return false;
			}
			var tokens = await _refreshTokenRepository.GetAllUserTokensAsync(userId);
			foreach (var token in tokens)
			{
				token.Revoke();
			}
			await _unitOfWork.SaveChangesAsync();
			_logger.LogInformation("All devices logged out for UserId: {UserId}", userId);
			return true;
		}

		public async Task<bool> ChangePasswordAsync(Guid userId , ChangePasswordDto dto)
		{
			_logger.LogInformation("Change password attempt started for UserId: {UserId}", userId);
			ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
			if (user==null)
			{
				_logger.LogWarning("Change password failed. User not found: {UserId}", userId);
				return false;
			}

			var identityResult = await _userManager.ChangePasswordAsync(user, dto.oldPassword, dto.newPassword);
			if (!identityResult.Succeeded)
			{
				_logger.LogWarning("Change password failed for UserId: {UserId}. Errors: {Errors}",	userId,	string.Join(", ", identityResult.Errors.Select(e => e.Description)));
				return false;
			}
			await _unitOfWork.SaveChangesAsync();
			var logoutResult =await LogoutAllDevicesAsync(user.Id);
			if (logoutResult==false)
			{
				return false;
			}
			_logger.LogInformation("Password changed successfully for UserId: {UserId}", userId);
			return true;

		}

		public async Task ForgotPasswordAsync(string email) 
		{
			_logger.LogInformation("Forgot password requested for Email: {Email}", email);
			if (string.IsNullOrWhiteSpace(email))
			{
				return;
			}
			ApplicationUser? user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				_logger.LogInformation("Forgot password flow completed");
				return;
			}
			var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedToken= WebUtility.UrlEncode(rawToken);
			var encodedEmail= WebUtility.UrlEncode(email);
			var BaseUrl = _configuration.GetValue<string>("ClientApp:BaseUrl");
			var resetlink = $"{BaseUrl}/reset-password?email={encodedEmail}&token={encodedToken}";
			var to = email;
			string Subject = "Reset Your Password";
			string Body = $"Click the link below to reset your password:\r\n<{resetlink}>\r\n";
			try 
			{
				await _emailService.SendEmailAsync(to, Subject, Body);
				_logger.LogInformation("Reset password email sent to {Email}", email);
			}
			catch (EmailSendingException ex)
			{
				_logger.LogError(ex, "Failed to send reset password email to {Email}", email);
			}
			return;
		}
		public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto dto)
		{
			_logger.LogInformation("Reset password attempt started for Email: {Email}", dto.Email);
			var DecodedToken = WebUtility.UrlDecode(dto.Token);
			ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);
			if (user==null)
			{
				_logger.LogWarning("Reset password failed. User not found: {Email}", dto.Email);
				return new ResetPasswordResultDto() {IsSuccess=false, Errors= new List<string> { "problem ocured"} };
			}
			IdentityResult result= await _userManager.ResetPasswordAsync(user, DecodedToken, dto.NewPassword);
			if (!result.Succeeded)
			{
				_logger.LogWarning("Reset password failed for Email: {Email}. Errors: {Errors}",dto.Email,string.Join(", ", result.Errors.Select(e => e.Description)));
				return new ResetPasswordResultDto() { IsSuccess = false, Errors = result.Errors.Select(e=>e.Description).ToList() };
			}

			await LogoutAllDevicesAsync(user.Id);
			_logger.LogInformation("Password reset successfully for UserId: {UserId}", user.Id);
			return new ResetPasswordResultDto() {IsSuccess=true };
		}

		public async Task SendEmailConfirmationAsync( string email)
		{
			_logger.LogInformation("Email confirmation requested for Email: {Email}", email);

			if (string.IsNullOrWhiteSpace(email))
			{
				return;
			}
			ApplicationUser? user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return;
			}

			string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			string encodedToken = WebUtility.UrlEncode(confirmationToken);
			string encodedEmail = WebUtility.UrlEncode(email);
			var BaseUrl = _configuration.GetValue<string>("ClientApp:BaseUrl");
			var confirmlink = $"{BaseUrl}/confirm-email?email={encodedEmail}&token={encodedToken}";
			var to = email;
			string Subject = "Confirm Your Email";
			string Body = $"Click the link below to Confirm your Email:\r\n<{confirmlink}>\r\n";
			try
			{
				await _emailService.SendEmailAsync(to, Subject, Body);
				_logger.LogInformation("Confirmation email sent to {Email}", email);
			}
			catch (EmailSendingException ex)
			{
				_logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
			}
			return;
		}

		public async Task<bool> ConfirmEmailAsync(ConfirmEmailDto dto)
		{
			_logger.LogInformation("Email confirmation attempt for Email: {Email}", dto.Email);

			var decodedToken = WebUtility.UrlDecode(dto.ConfirmationToken);
			ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null) 
			{
				_logger.LogWarning("Email confirmation failed. User not found: {Email}", dto.Email);
				return false;
			}
			if (user.EmailConfirmed == true)
			{
				_logger.LogInformation("Email already confirmed for UserId: {UserId}", user.Id);
				return true;
			}
			IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);
			if (!result.Succeeded)
			{
				_logger.LogWarning("Email confirmation failed for UserId: {UserId}", user.Id);
				return false;
			}
			_logger.LogInformation("Email confirmed successfully for UserId: {UserId}", user.Id);
			return true;
		}
	}
}
