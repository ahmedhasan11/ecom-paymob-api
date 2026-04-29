using Ecom.Application.DTOs.Cart;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly ICurrentUserService _currentUserService;

		public CartController(ICartService cartService, ICurrentUserService currentUserService)
		{
			_cartService = cartService;
			_currentUserService = currentUserService;
		}

		[HttpGet]
		public async Task<ActionResult<CartResultDto>> GetCart( CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			CartResultDto cart = await _cartService.GetMyCartAsync(userId.Value, cancellationToken);
			return Ok(cart);
		}

		[HttpPost("items/add")]
		public async Task<ActionResult<CartResultDto>> AddItemToCart(RequestAddToCartDto dto, CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}

			CartResultDto cart = await _cartService.AddItemToCartAsync(userId.Value, dto, cancellationToken);

			return Ok(cart);
		}

		[HttpDelete("items/{productId}")]
		public async Task<ActionResult<CartResultDto>> RemoveItemFromCart(Guid productId, CancellationToken cancellationToken)
		{
			if (productId == Guid.Empty)
			{
				return BadRequest();
			}
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			CartResultDto cart = await _cartService.RemoveItemFromCartAsync(userId.Value, productId, cancellationToken);
			return Ok(cart);
		}

		[HttpPatch("items/{productId}")]
		public async Task<ActionResult<CartResultDto>> UpdateCartItemQuantity(Guid productId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken)
		{
			if (productId == Guid.Empty)
			{
				return BadRequest();
			}
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}

			CartResultDto cart = await _cartService.UpdateCartItemQuantityAsync(userId.Value, productId, dto, cancellationToken);
			return Ok(cart);
		}

		[HttpDelete("items")]
		public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			await _cartService.ClearCartAsync(userId.Value, cancellationToken);
			return NoContent();
		}
	}
}
