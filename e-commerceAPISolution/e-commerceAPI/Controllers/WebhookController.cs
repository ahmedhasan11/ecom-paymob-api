using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebhookController : ControllerBase
	{
		private readonly IPaymentWebhookService _paymentWebhookService;
		public WebhookController(IPaymentWebhookService paymentWebhookService)
		{
			_paymentWebhookService = paymentWebhookService;
		}

		[HttpPost("paymob")]
		public async Task<IActionResult> HandlePaymentWebhook([FromBody]PaymentWebhookRequest req, [FromQuery(Name = "hmac")] string hmac, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(hmac))
			{
				return BadRequest("Missing hmac");
			}
			await _paymentWebhookService.HandleWebhookAsync(req, hmac, cancellationToken);
			return Ok();
		}
	}
}
