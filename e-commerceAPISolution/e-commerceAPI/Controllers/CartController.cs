using Ecom.Application.DTOs.Cart;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
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
		public async Task<ActionResult<CartResultDto>> GetCart( CancellationToken cancellationToken)
		{
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}
			CartResultDto cart = await _cartService.GetMyCartAsync(userId, cancellationToken);
			return Ok(cart);
		}

		[HttpPost("items/add")]
		public async Task<ActionResult<CartResultDto>> AddItemToCart(RequestAddToCartDto dto, CancellationToken cancellationToken)
		{
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}

			CartResultDto cart = await _cartService.AddItemToCartAsync(userId, dto,cancellationToken);

			return Ok(cart);
		}

		[HttpDelete("items/{productId}")]
		public async Task<ActionResult<CartResultDto>> RemoveItemFromCart(Guid productId, CancellationToken cancellationToken)
		{
			if (productId==Guid.Empty)
			{
				return BadRequest();
			}
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}
			CartResultDto cart = await _cartService.RemoveItemFromCartAsync(userId,productId,cancellationToken );
			return Ok(cart);
		}
		[HttpPatch("items/{productId}")]
		public async Task<ActionResult<CartResultDto>> UpdateCartItemQuantity(Guid productId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken)
		{
			if (productId == Guid.Empty)
			{
				return BadRequest();
			}
			if (dto.Quantity < 0)
			{
				return BadRequest();
			}
			var Id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if (!Guid.TryParse(Id, out var userId))
			{
				return Unauthorized();
			}

			CartResultDto cart = await _cartService.UpdateCartItemQuantityAsync(userId, productId, dto,cancellationToken);
			return Ok(cart);
		}

		[HttpDelete("items")]
		public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
		{
			var id = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
			if(!Guid.TryParse(id, out var userId))
			{
				return Unauthorized();
			}
			await _cartService.ClearCartAsync(userId, cancellationToken);
			return NoContent();
		}
	}
}
