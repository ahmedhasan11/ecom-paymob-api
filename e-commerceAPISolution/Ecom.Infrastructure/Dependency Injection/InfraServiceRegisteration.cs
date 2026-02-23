using Ecom.Application.Common.Settings;
using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using Ecom.Infrastructure.Authentication_Services;
using Ecom.Infrastructure.Caching;
using Ecom.Infrastructure.Identity;
using Ecom.Infrastructure.Persistence;
using Ecom.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Dependency_Injection
{
	public static class InfraServiceRegisteration
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
		{
			services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));
			services.AddIdentity<ApplicationUser, ApplicationRole>(options => 
			{
				options.Password.RequiredLength = 5;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;//symbols
				options.Password.RequireLowercase = true;
				options.Password.RequireDigit = false;
				//options.Password.RequiredUniqueChars = 3;
			})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); //lockout time 
				options.Lockout.MaxFailedAccessAttempts = 5; //max n of failed attempts
				options.Lockout.AllowedForNewUsers = true; // new account is able to be locked
			});

			services.AddMemoryCache();
			services.AddSingleton<RedisConnectionFactory>();
			services.AddSingleton<ICacheService, RedisCacheService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IRefreshTokenService, RefreshTokenService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IdentityDbInitializer>();
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.Configure<AdminUserSettings>(configuration.GetSection("AdminUser"));
			return services;
		}
	}
}
