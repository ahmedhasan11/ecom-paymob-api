using Ecom.Application.DTOs.Authentication;
using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
		public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService
			, IRefreshTokenRepository refreshTokenRepository, IRefreshTokenService refreshTokenService, IConfiguration configuration, IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_jwtService = jwtService;
			_roleManager = roleManager;
			_refreshTokenRepository = refreshTokenRepository;
			_refreshTokenService = refreshTokenService;
			_unitOfWork = unitOfWork;
			_configuration = configuration;
		}
	
		public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
		{
			if (await IsEmailAlreadyRegistered(dto.Email!))
			{
				return new AuthResponseDto() { IsSuccess=false , Errors= new List<string> { "Email Is Already Registered"} };
			}  

			ApplicationUser user = new ApplicationUser() { FullName=dto.FullName! , PhoneNumber=dto.PhoneNumber,
				Email= dto.Email, UserName= dto.Email, CreatedAt= DateTime.UtcNow};

			var result =await _userManager.CreateAsync(user, dto.Password!);	 
			if (!result.Succeeded)
			{
				return new AuthResponseDto() { IsSuccess = false, Errors= result.Errors.Select(e=>e.Description) };
			}

			await _userManager.AddToRoleAsync(user, "User");
			var roles = await _userManager.GetRolesAsync(user);

			JwtUserDataDto jwtUserDataDto = new JwtUserDataDto() { UserId=user.Id, Email= user.Email! , FullName= user.FullName , Roles= roles}; 
			JwtResultDto jwtresult= await _jwtService.GenerateTokenAsync(jwtUserDataDto);

			var session = await IssueNewSessionAsync(user.Id);
			return new AuthResponseDto() { IsSuccess = true , Token = jwtresult.Token, RefreshToken= session.RawToken, ExpiresAt = jwtresult.ExpiresAt };
		}
		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email!);
			if (user==null)
			{
				return new AuthResponseDto() { IsSuccess=false, Errors= new List<string>() { "Email Or Password is Invalid" } };
			}
			var passwordcheck = await _userManager.CheckPasswordAsync(user, dto.Password!);
			if (!passwordcheck)
			{
				return new AuthResponseDto() { IsSuccess = false, Errors = new List<string>() { "Email Or Password is Invalid" } };
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
			int lifetimeDays = _configuration.GetValue<int>("RefreshToken:RefreshTokenLifetimeDays");
			DateTime absoluteExpirationTime = DateTime.UtcNow.AddDays(lifetimeDays);

			RefreshTokenResultDto refreshTokenresult =  _refreshTokenService.GenerateRefreshTokenAsync(absoluteExpirationTime);

			RefreshToken refreshEntity = new RefreshToken(userId, refreshTokenresult.HashedToken, refreshTokenresult.ExpiresAt);
			await _refreshTokenRepository.AddRefreshTokenAsync(refreshEntity);
			await _unitOfWork.SaveChangesAsync();

			return refreshTokenresult;
		}
		public async Task<AuthResponseDto> RefreshSessionAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
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
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}
			if (refreshtokenfrom_db.IsRevoked == true)
			{
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
				return new AuthResponseDto
				{
					IsSuccess = false,
					Errors = new List<string> { "Invalid session." }
				};
			}
			var roles = await _userManager.GetRolesAsync(user);
			JwtUserDataDto jwtUserDataDto = new JwtUserDataDto() { UserId = user.Id, Email = user.Email!, FullName = user.FullName, Roles = roles };
			JwtResultDto jwtresult = await _jwtService.GenerateTokenAsync(jwtUserDataDto); 
			#endregion

			DateTime absoluteExpirationTime = refreshtokenfrom_db.ExpiresAt; //keep same absolute expiration

			RefreshTokenResultDto refreshTokenresult =  _refreshTokenService.GenerateRefreshTokenAsync(absoluteExpirationTime);
			refreshtokenfrom_db.Revoke();
			RefreshToken refreshEntity = new RefreshToken(user.Id, refreshTokenresult.HashedToken, refreshTokenresult.ExpiresAt);
			await _refreshTokenRepository.AddRefreshTokenAsync(refreshEntity);
			await _unitOfWork.SaveChangesAsync();
			return new AuthResponseDto() { IsSuccess = true, Token = jwtresult.Token, RefreshToken = refreshTokenresult.RawToken, ExpiresAt = jwtresult.ExpiresAt };
		}

		public async Task<bool> LogoutDeviceAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				return false;
			}
			string HashedToken =_refreshTokenService.HashToken(refreshToken);
			RefreshToken? tokenfromdb = await _refreshTokenRepository.GetByHashedTokenAsync(HashedToken);

			if (tokenfromdb==null)
			{
				return false;
			}
			tokenfromdb.Revoke();
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<bool> LogoutAllDevicesAsync(Guid userId)
		{
			if (userId==Guid.Empty)
			{
				return false;
			}
			var tokens = await _refreshTokenRepository.GetAllUserTokensAsync(userId);
			foreach (var token in tokens)
			{
				token.Revoke();
			}
			await _unitOfWork.SaveChangesAsync();
			return true;
		}
	}
}
