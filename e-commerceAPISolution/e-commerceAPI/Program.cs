using Ecom.Application.Dependency_Injection;
using Ecom.Infrastructure.Dependency_Injection;
using Ecom.Infrastructure.Identity;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
