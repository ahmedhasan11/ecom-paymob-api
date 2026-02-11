
using Ecom.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace e_commerceAPI.Middlewares
{
	public class GlobalExceptionMiddleware : IMiddleware
	{
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware( ILogger<GlobalExceptionMiddleware> logger)
		{
			_logger = logger;

		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
				#region Notes
				/*"خلّي الطلب يكمل لباقي السيستم"

		يعني روح للـ Controllers والـ Services وكل حاجة.

		لو مفيش خطأ → الميدل وير مالوش دور تاني.*/ 
				#endregion
			}
			catch (Exception ex)
			{
				int statusCode;
				string title;
				switch (ex)
				{
					case ValidationException:
						statusCode = StatusCodes.Status400BadRequest;
						title = "Validation Failed";
						break;

					case NotFoundException:
						statusCode = StatusCodes.Status404NotFound;
						title = "Resource Not Found";
						break;

					case UnauthorizedAccessException:
						statusCode = StatusCodes.Status401Unauthorized;
						title = "Unauthorized access";
						break;

					default:
						statusCode = StatusCodes.Status500InternalServerError;
						title = "Internal Server error";
						break;

				}
				//if an error hhappened at any layer
				_logger.LogError(ex, "Unhandled exception occurred while processing {Method} {Path}. TraceId={TraceId}",
				context.Request.Method,	context.Request.Path, context.TraceIdentifier);

				context.Response.ContentType = "application/problem+json";
				context.Response.StatusCode = statusCode;

				var Problem = new ProblemDetails()
				{
					Status = statusCode,
					Title = title,
					Detail = ex.Message,
					Instance = context.Request.Path,										
				};
				Problem.Extensions["TraceId"] = context.TraceIdentifier;
				await context.Response.WriteAsJsonAsync(Problem);
			}

		}
	}
}
