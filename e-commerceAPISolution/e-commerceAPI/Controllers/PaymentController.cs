using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		private readonly ICurrentUserService _currentUserService;

		public PaymentController(IPaymentService paymentService, ICurrentUserService currentUserService)
		{
			_paymentService = paymentService;
			_currentUserService = currentUserService;
		}

		[HttpPost("{orderId}/session")]
		public async Task<ActionResult<PaymentSessionResponse>> CreatePaymentSession(Guid orderId, CancellationToken cancellationToken)
		{
			if (orderId == Guid.Empty)
			{
				return BadRequest("orderId cannot be empty.");
			}
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			var paymentSessionResponse = await _paymentService.CreatePaymentSessionAsync(orderId, userId.Value, cancellationToken);
			
			return Ok(paymentSessionResponse);
		}

	}
}
