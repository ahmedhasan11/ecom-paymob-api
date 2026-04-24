using Ecom.Application.Common.Settings;
using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using Ecom.Infrastructure.Authentication_Services;
using Ecom.Infrastructure.Caching;
using Ecom.Infrastructure.Common.Settings;
using Ecom.Infrastructure.Identity;
using Ecom.Infrastructure.Payments;
using Ecom.Infrastructure.Persistence;
using Ecom.Infrastructure.Persistence.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
			services.AddScoped<IdentityDbInitializer>();

			//Repositories
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<ICartRepository, CartRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IReservationRepository, ReservationRepository>();
			services.AddScoped<IPaymentRepository, PaymentRepository>();

			//External API Services
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IRefreshTokenService, RefreshTokenService>();
			services.AddScoped<IEmailService, EmailService>();

			//Paymob Configurations
			services.AddScoped<IPaymobHmacValidator, PaymobHmacValidator>();
			services.AddScoped<IPaymentGateway, PaymentGateway>();
			services.AddScoped<IPaymentConfiguration, PaymentConfiguration>();

			//IOptions Settings Configurations
			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.Configure<AdminUserSettings>(configuration.GetSection("AdminUser"));
			services.Configure<PaymobSettings>(configuration.GetSection("Paymob"));

			//Paymob Call Configure
			services.AddHttpClient<PaymentGateway>(client =>
			{
				client.BaseAddress = new Uri(configuration["Paymob:BaseUrl"]!);
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token",	configuration["Paymob:SecretKey"]
		);
			});

			//Hangfire Configuration
			services.AddHangfire(config => config
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnectionString")));

			// Add global retry logic for Hangfire jobs
			GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
			{
				Attempts = 1,
				DelaysInSeconds = new[] { 60, 120 },
				OnAttemptsExceeded = AttemptsExceededAction.Fail
			});

			services.AddHangfireServer();
			return services;
		}
	}
}
