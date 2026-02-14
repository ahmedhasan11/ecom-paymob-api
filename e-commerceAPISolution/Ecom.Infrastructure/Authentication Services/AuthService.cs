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
		private readonly RoleManager _roleManager;
		public AuthService(UserManager<ApplicationUser> userManager , RoleManager roleManager)
		{
			 _roleManager = roleManager;
			_userManager = userManager;
		}
		public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
		{
			throw new NotImplementedException();
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
		{
			throw new NotImplementedException();
		}
	}
}
