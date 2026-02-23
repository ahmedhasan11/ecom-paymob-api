using Ecom.Application.Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Identity
{
	public  class IdentityDbInitializer
	{
		private readonly AdminUserSettings _adminSettings;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		public IdentityDbInitializer(IOptions<AdminUserSettings> adminsettings, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
		{
			_adminSettings = adminsettings.Value;
			_roleManager = roleManager;
			_userManager = userManager;
		}
		public async Task SeedRolesAsync()
		{
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
			}
			if (!await _roleManager.RoleExistsAsync("User"))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "User" });
			}
		}

		public async Task SeedAdminUserAsync()
		{
			var email = _adminSettings.Email;
			var password= _adminSettings.Password;
			var name = _adminSettings.FullName;
			var user= await _userManager.FindByEmailAsync(email);
			if (user==null)
			{
				 user = new ApplicationUser() { Email=email, FullName= name , EmailConfirmed=true};
				var result = await _userManager.CreateAsync(user, password);

				if (!result.Succeeded)
				{
					throw new Exception("Failed to create admin user");
				}

			}
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
			}

			if (!await _userManager.IsInRoleAsync(user, "Admin"))
			{
				await _userManager.AddToRoleAsync(user, "Admin");
			}

			if (!user.EmailConfirmed)
			{
				user.EmailConfirmed = true;
				await _userManager.UpdateAsync(user);
			}
		}
	}
}
