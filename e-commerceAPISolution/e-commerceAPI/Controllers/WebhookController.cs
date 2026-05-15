using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebhookController : ControllerBase
	{
		private readonly IPaymentWebhookService _paymentWebhookService;
		private readonly ILogger<WebhookController> _logger;
		public WebhookController(IPaymentWebhookService paymentWebhookService, ILogger<WebhookController> logger)
		{
			_paymentWebhookService = paymentWebhookService;
			_logger = logger;
		}

		[HttpPost("paymob")]
		[AllowAnonymous]
		public async Task<IActionResult> HandlePaymentWebhook([FromBody]PaymentWebhookRequest req, [FromQuery(Name = "hmac")] string hmac, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(hmac))
			{
				return BadRequest("Missing hmac");
			}
			try
			{
				await _paymentWebhookService.HandleWebhookAsync(req, hmac, CancellationToken.None);
				return Ok(); // ✅ success
			}
			catch (InvalidHmacException ex)
			{
				_logger.LogWarning(ex, "Invalid HMAC received");
				return StatusCode(500);
			}
		}
	}
}
