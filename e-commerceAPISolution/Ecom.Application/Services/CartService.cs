using Ecom.Application.DTOs.Cart;
using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class CartService:ICartService
	{
		private readonly ICartRepository _cartRepository;
		private readonly ILogger<CartService> _logger;
		public CartService(ICartRepository cartRepository, ILogger<CartService> logger) 
		{
			_cartRepository=cartRepository;
			_logger=logger;
		}

		public async Task<CartResultDto> GetMyCartAsync(Guid userId)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("UserId cannot be empty.", nameof(userId));
			}

			var query = _cartRepository.GetMyCartAsyc(userId);

			CartResultDto? cart = await query.Select(c => new CartResultDto
			{
				CartId = c.Id,
			    TotalItemsCount= c.CartItems.Select(ci=>ci.Quantity).DefaultIfEmpty(0).Sum(),
			    SubTotal= c.CartItems.Select(ci => ci.Quantity * ci.UnitPrice).DefaultIfEmpty(0m).Sum(),
			    HasUnavailableItems= c.CartItems.Any(ci=>ci.Product.IsDeleted||!ci.Product.IsAvailable||ci.Product.StockQuantity<=0),
				CartItems= c.CartItems.Select(ci=>new CartItemDto
				{
					 ProductId=ci.ProductId,
					 Quantity=ci.Quantity,
					 ProductName=ci.Product.Name,
					 UnitPrice= ci.UnitPrice,
					 Total= ci.Quantity*ci.UnitPrice,
					 IsAvailable = (ci.Product.IsAvailable&& !ci.Product.IsDeleted && ci.Product.StockQuantity>0)
				}).ToList()			
			}).FirstOrDefaultAsync();

			if (cart==null)
			{
				return new CartResultDto();
			}

			return cart;
		}
	}
}
