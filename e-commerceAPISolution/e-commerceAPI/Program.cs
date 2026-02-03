using Ecom.Application.Dependency_Injection;
using Ecom.Infrastructure.Dependency_Injection;
using Microsoft.AspNetCore.Mvc;
namespace e_commerceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Extension Methods DI
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();


            builder.Services.AddControllers();
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
            // Configure the HTTP request pipeline.
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
