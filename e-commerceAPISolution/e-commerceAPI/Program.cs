using Ecom.Application.Dependency_Injection;
using Ecom.Infrastructure.Dependency_Injection;
using Ecom.Infrastructure.Identity;
using FluentValidation.AspNetCore;
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

			});

			//Extension Methods DI
			builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

			var jwtSettings = builder.Configuration.GetSection("Jwt");

			builder.Services.AddRateLimiter(options =>
			{
				options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
					RateLimitPartition.GetSlidingWindowLimiter(
						partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
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
						partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
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
						partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
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
					Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
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
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
				await IdentityDbInitializer.SeedRolesAsync(roleManager);
			}

			app.UseSerilogRequestLogging();
			app.UseHttpLogging();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
			app.UseRateLimiter();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
            app.Run();
        }
    }
}
