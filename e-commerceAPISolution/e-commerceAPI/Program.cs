using e_commerceAPI.Filters;
using e_commerceAPI.Hangfire;
using e_commerceAPI.Middlewares;
using Ecom.Application.Dependency_Injection;
using Ecom.Infrastructure.Dependency_Injection;
using Ecom.Infrastructure.Identity;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

namespace e_commerceAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service, LoggerConfiguration logger_configuration) =>
			{
				logger_configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(service);
				logger_configuration.Enrich.FromLogContext().WriteTo.Console();
			});
			builder.Services.AddHttpLogging(options => {
				options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders
					|
					Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
				// Security: Prevent sensitive header logging
				options.RequestHeaders.Remove("Authorization");
				options.RequestHeaders.Remove("Cookie");

			});

			//Extension Methods DI
			builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

			var jwtSettings = builder.Configuration.GetSection("Jwt");
			var secretKey = jwtSettings["Secret"];
			if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
			{
				throw new InvalidOperationException("JWT Secret is missing or too short (minimum 32 characters required for HS256).");
			}

			builder.Services.AddRateLimiter(options =>
			{
				options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
					RateLimitPartition.GetSlidingWindowLimiter(
						partitionKey: GetPartitionKey(context, allowUserId: true),
						factory: _ => new SlidingWindowRateLimiterOptions
						{
							PermitLimit = 100,
							Window = TimeSpan.FromMinutes(1),
							SegmentsPerWindow = 4,
							QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
							QueueLimit = 0
						}));

				options.AddPolicy("LoginPolicy", context =>
					RateLimitPartition.GetSlidingWindowLimiter(
						partitionKey: GetPartitionKey(context, allowUserId: false),
						factory: _ => new SlidingWindowRateLimiterOptions
						{
							PermitLimit = 5,
							Window = TimeSpan.FromMinutes(1),
							SegmentsPerWindow = 2,
							QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
							QueueLimit = 0
						}));

				options.AddPolicy("ForgotPolicy", context =>
					RateLimitPartition.GetSlidingWindowLimiter(
						partitionKey: GetPartitionKey(context, allowUserId: false),
						factory: _ => new SlidingWindowRateLimiterOptions
						{
							PermitLimit = 3,
							Window = TimeSpan.FromMinutes(10),
							SegmentsPerWindow = 2,
							QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
							QueueLimit = 0
						}));

				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
			});

			static string GetPartitionKey(HttpContext context, bool allowUserId)
			{
				if (allowUserId)
				{
					// Use "sub" claim specifically as requested
					var userId = context.User?.FindFirst("sub")?.Value;
					if (!string.IsNullOrEmpty(userId))
					{
						return $"user:{userId}";
					}
				}

				// Fallback to IP address
				var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
				return $"ip:{ipAddress}";
			}


			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,

					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(secretKey))
				};
			});

			builder.Services.AddAuthorization(options =>
			{ //so now any endpoint that dont have [AllowAnonymous] will be [Authorize] by default
				options.FallbackPolicy = new AuthorizationPolicyBuilder() //FallBackPolicy y3ny endpoint m4 3leh [Authorize] aw [AllowAnonymous]
						.RequireAuthenticatedUser() //so here we says if there is endpopint have nothing , make the user authenticated on it
						.Build();

				options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); 
				//here framework reads the jwt token sent , check claims if there is claim --> role= Admin
			});

			builder.Services.AddControllers();

			builder.Services.AddFluentValidationAutoValidation(); /*?? ???? HTTP request ???? FluentValidation ????????*/

			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

			builder.Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var problemDetails = new ValidationProblemDetails(context.ModelState)
					{
						Type = "https://httpstatuses.com/400",
						Title = "Validation errors occurred.",
						Status = StatusCodes.Status400BadRequest,
						Instance = context.HttpContext.Request.Path
					};
					problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

					return new BadRequestObjectResult(problemDetails)
					{
						ContentTypes = { "application/problem+json" }
					};
				};
			});

			var app = builder.Build();
			//DB seeding
			using (var scope = app.Services.CreateScope())
			{
				var initializer = scope.ServiceProvider	.GetRequiredService<IdentityDbInitializer>();
				await initializer.SeedRolesAsync();
				await initializer.SeedAdminUserAsync();
			}

			//Global Exception Handling
			app.UseMiddleware<GlobalExceptionMiddleware>();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			//Logging
			app.UseSerilogRequestLogging();
			app.UseHttpLogging();


            app.UseHttpsRedirection();
			app.UseRouting();// Required for RateLimiter and Auth to acknowledge endpoints
			app.UseRateLimiter();// Security: Throttling before Auth to prevent resource exhaustion

			//Auth
			app.UseAuthentication();
			app.UseAuthorization();

			//Hangfire
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization = new[] { new HangfireAuthorizationFilter() }
			});
			app.AddHangfireJobs();

			app.MapControllers();

			app.Run();
        }
    }
}
