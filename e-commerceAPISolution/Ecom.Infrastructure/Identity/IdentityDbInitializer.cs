using Ecom.Application.Common.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger<IdentityDbInitializer> _logger;
		public IdentityDbInitializer(IOptions<AdminUserSettings> adminsettings, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager , ILogger<IdentityDbInitializer> logger)
		{
			_adminSettings = adminsettings.Value;
			_roleManager = roleManager;
			_userManager = userManager;
			_logger = logger;
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
			_logger.LogInformation("Starting admin user seeding process");
			var email = _adminSettings.Email;
			var password= _adminSettings.Password;
			var name = _adminSettings.FullName;
			var user= await _userManager.FindByEmailAsync(email);
			if (user==null)
			{
				_logger.LogInformation("Admin user not found. Creating new admin user with Email: {Email}", email);
				user = new ApplicationUser() { Email=email, FullName= name , EmailConfirmed=true};
				var result = await _userManager.CreateAsync(user, password);

				if (!result.Succeeded)
				{
					_logger.LogError("Failed to create admin user. Errors: {Errors}",string.Join(", ", result.Errors.Select(e => e.Description)));
					throw new Exception("Failed to create admin user");
				}
				_logger.LogInformation("Admin user created successfully with Email: {Email}", email);
			}
			else
			{
				_logger.LogInformation("Admin user already exists with Email: {Email}", email);
			}
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				_logger.LogWarning("Admin role does not exist. Creating Admin role");
				await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
				_logger.LogInformation("Admin role created successfully");
			}

			if (!await _userManager.IsInRoleAsync(user, "Admin"))
			{
				_logger.LogInformation("Assigning Admin role to user: {Email}", email);
				await _userManager.AddToRoleAsync(user, "Admin");
				_logger.LogInformation("Admin role assigned successfully to user: {Email}", email);
			}

			if (!user.EmailConfirmed)
			{
				_logger.LogInformation("Admin email not confirmed. Confirming email for: {Email}", email);
				user.EmailConfirmed = true;
				await _userManager.UpdateAsync(user);
				_logger.LogInformation("Admin email confirmed successfully for: {Email}", email);
			}
			_logger.LogInformation("Admin seeding process completed successfully");
		}
	}
}
