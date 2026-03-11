using Ecom.Application.DTOs.Order;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CheckoutController : ControllerBase
	{
		private readonly ICheckoutService _checkoutService;
		public CheckoutController(ICheckoutService checkoutService)
		{
			_checkoutService = checkoutService;
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> Checkout(ShippingAddressDto addressDto,CancellationToken cancellationToken)
		{
			var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(id, out var userId))
			{
				return Unauthorized();
			}
			var orderId = await _checkoutService.CheckoutAsync(userId, cancellationToken, addressDto);

			return Ok(new { orderId });
		}
	}
}
