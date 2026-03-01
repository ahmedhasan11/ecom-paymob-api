using Ecom.Application.DTOs.Cart;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		public CartController(ICartService cartService)
		{
		_cartService = cartService;
		}


		[HttpGet]
		public async Task<ActionResult<CartResultDto>> GetCart()
		{
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if(!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}
			CartResultDto cart =await _cartService.GetMyCartAsync(userId);
			return Ok(cart);
		}
	}
}
