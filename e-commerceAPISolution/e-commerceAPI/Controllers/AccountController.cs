using Ecom.Application.DTOs.Authentication;
using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AccountController( IAuthService authService)
		{ 
			_authService = authService; 		
		}

		[HttpPost("register")]
		public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
		{

			var result = await _authService.RegisterAsync(dto);
			if (result.IsSuccess==false)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("login")]

		public async Task<ActionResult<AuthResponseDto>> Login (LoginDto dto)
		{

			var result = await _authService.LoginAsync(dto);
			if (result.IsSuccess==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[HttpPost("refresh")]
		public async Task<ActionResult<AuthResponseDto>> RefreshSession(RefreshRequestDto dto)
		{
			AuthResponseDto result= await _authService.RefreshSessionAsync(dto.RefreshToken);
			if (!result.IsSuccess)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout(LogoutRequestDto dto)
		{
			bool result = await _authService.LogoutDeviceAsync(dto.refreshToken);
			if (result==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);
		}
		[Authorize]
		[HttpPost("logout-all")]
		public async Task<IActionResult> LogoutAll()
		{
			var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value; //byrg3 string , method el service mstnya Guid
			if (!Guid.TryParse(userIdClaim, out var userId)) //TryParse 3shan lw Guid.Parse lw wrong value --> exception
			{
				return Unauthorized();
			}
			var result = await _authService.LogoutAllDevicesAsync(userId);
			if (result==false)
			{
				return Unauthorized(result);
			}
			return Ok(result);

		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
		{
			var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if(!Guid.TryParse(userIdClaim, out var userId))
			{
				return Unauthorized();
			}
			bool result = await _authService.ChangePasswordAsync(userId, dto);
			if (result==false)
			{
				return BadRequest("Old password is incorrect.");
			}
			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
		{
			await _authService.ForgotPasswordAsync(dto.Email);
			return Ok(new
			{
				message = "If your email exists, you will receive a reset link."
			});
		}

		[AllowAnonymous]
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
		{
			ResetPasswordResultDto result=  await _authService.ResetPasswordAsync(dto);
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
		public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
		{
			bool isconfirmed = await _authService.ConfirmEmailAsync(dto);
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
