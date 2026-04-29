using Ecom.Application.DTOs.Order;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CheckoutController : ControllerBase
	{
		private readonly ICheckoutService _checkoutService;
		private readonly ICurrentUserService _currentUserService;

		public CheckoutController(ICheckoutService checkoutService, ICurrentUserService currentUserService)
		{
			_checkoutService = checkoutService;
			_currentUserService = currentUserService;
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> Checkout(ShippingAddressDto addressDto,CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			var orderId = await _checkoutService.CheckoutAsync(userId.Value, cancellationToken, addressDto);

			return Ok(new { orderId });
		}
	}
}
