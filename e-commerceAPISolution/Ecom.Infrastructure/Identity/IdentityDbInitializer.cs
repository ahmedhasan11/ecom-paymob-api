using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Identity
{
	public static class IdentityDbInitializer
	{
		public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
		{
			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
			}
			if (!await roleManager.RoleExistsAsync("User"))
			{
				await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
			}
		}
	}
}
