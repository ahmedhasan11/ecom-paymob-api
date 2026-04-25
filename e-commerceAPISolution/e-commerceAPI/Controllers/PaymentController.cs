using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		public PaymentController(IPaymentService paymentService)
		{
			_paymentService = paymentService;
		}

		
		[HttpPost("{orderId}/session")]
		public async Task<ActionResult<PaymentSessionResponse>> CreatePaymentSession(Guid orderId, CancellationToken cancellationToken)
		{
			if (orderId == Guid.Empty)
			{
				return BadRequest("orderId cannot be empty.");
			}
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}
			var paymentSessionResponse= await _paymentService.CreatePaymentSessionAsync(orderId,userId, cancellationToken);
			
			return Ok(paymentSessionResponse);
		}

	}
}
