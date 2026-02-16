using Ecom.Application.DTOs.Authentication;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

	}
}
