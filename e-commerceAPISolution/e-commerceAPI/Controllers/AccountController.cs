using Ecom.Application.DTOs.Authentication;
using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;

namespace e_commerceAPI.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AccountController( IAuthService authService)	{_authService = authService;}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto, CancellationToken cancellationToken)
		{

			var result = await _authService.RegisterAsync(dto, cancellationToken);
			if (result.IsSuccess==false)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[EnableRateLimiting("LoginPolicy")]
		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> Login (LoginDto dto, CancellationToken cancellationToken)
		{

			var result = await _authService.LoginAsync(dto, cancellationToken);
			if (result.IsSuccess==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[AllowAnonymous]
		[HttpPost("refresh")]
		public async Task<ActionResult<AuthResponseDto>> RefreshSession(RefreshRequestDto dto, CancellationToken cancellationToken)
		{
			AuthResponseDto result= await _authService.RefreshSessionAsync(dto.RefreshToken, cancellationToken);
			if (!result.IsSuccess)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout(LogoutRequestDto dto, CancellationToken cancellationToken)
		{
			bool result = await _authService.LogoutDeviceAsync(dto.refreshToken, cancellationToken);
			if (result==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);
		}

		[HttpPost("logout-all")]
		public async Task<IActionResult> LogoutAll( CancellationToken cancellationToken)
		{
			var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value; //byrg3 string , method el service mstnya Guid
			if (!Guid.TryParse(userIdClaim, out var userId)) //TryParse 3shan lw Guid.Parse lw wrong value --> exception
			{
				return Unauthorized();
			}
			var result = await _authService.LogoutAllDevicesAsync(userId, cancellationToken);
			if (result==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
		{
			var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if(!Guid.TryParse(userIdClaim, out var userId))
			{
				return Unauthorized();
			}
			bool result = await _authService.ChangePasswordAsync(userId, dto, cancellationToken);
			if (result==false)
			{
				return BadRequest("Old password is incorrect.");
			}
			return Ok(result);
		}


		[EnableRateLimiting("ForgotPolicy")]
		[AllowAnonymous]
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken cancellationToken)
		{
			await _authService.ForgotPasswordAsync(dto.Email, cancellationToken);
			return Ok(new
			{
				message = "If your email exists, you will receive a reset link."
			});
		}

		[AllowAnonymous]
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken cancellationToken)
		{
			ResetPasswordResultDto result=  await _authService.ResetPasswordAsync(dto, cancellationToken);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(new
			{
				message = "Password has been reset successfully. Please login again."
			});
		}

		[AllowAnonymous]
		[HttpPost("confirm-email")]
		public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto, CancellationToken cancellationToken)
		{
			bool isconfirmed = await _authService.ConfirmEmailAsync(dto, cancellationToken);
			if (isconfirmed==false)
			{
				return BadRequest(new
				{
					message = "Invalid or expired confirmation link."
				});
			}
			return Ok(new
			{
				message = "Email confirmed successfully."
			});

		}

	}
}
