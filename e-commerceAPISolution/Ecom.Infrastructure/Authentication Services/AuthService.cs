using Ecom.Application.DTOs.Authentication;
using Ecom.Application.Interfaces;
using Ecom.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IJwtService _jwtService;
		public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService )
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtService = jwtService;
			_roleManager = roleManager;
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

			return new AuthResponseDto() { IsSuccess = true , Token = jwtresult.Token, ExpiresAt = jwtresult.ExpiresAt };
		}
		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			throw new NotImplementedException();
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

	}
}
