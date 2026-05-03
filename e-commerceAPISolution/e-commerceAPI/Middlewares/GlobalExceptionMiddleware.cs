
using Ecom.Application.Exceptions;
using Ecom.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

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
					case DomainValidationException:
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
					case EmailSendingException:
						statusCode = StatusCodes.Status500InternalServerError;
						title = "Email service is currently unavailable.";
						break;
					case BusinessException:
						statusCode = StatusCodes.Status409Conflict;
						title = "Business rule violation";
						break;
					default:
						statusCode = StatusCodes.Status500InternalServerError;
						title = "Internal Server error";
						break;

				}
				//if an error hhappened at any layer
				var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;
				_logger.Log(logLevel, ex, "Exception occurred while processing {Method} {Path}. StatusCode={StatusCode}, TraceId={TraceId}",
				context.Request.Method, context.Request.Path, statusCode, context.TraceIdentifier);

				context.Response.ContentType = "application/problem+json";
				context.Response.StatusCode = statusCode;

				var problem = new ProblemDetails()
				{
					Status = statusCode,
					Title = title,
					Detail = statusCode == 500? "An unexpected error occurred": ex.Message,
					Instance = context.Request.Path,										
				};
				problem.Extensions["TraceId"] = context.TraceIdentifier;
				await context.Response.WriteAsJsonAsync(problem);
			}

		}
	}
}
