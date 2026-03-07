using Ecom.Application.DTOs.Cart;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
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
		private readonly IProductRepository _productRepository;
		private readonly ILogger<CartService> _logger;
		private readonly IUnitOfWork _unitOfWork;
		public CartService(ICartRepository cartRepository, ILogger<CartService> logger, IProductRepository productRepository, IUnitOfWork unitOfWork) 
		{
			_cartRepository=cartRepository;
			_logger=logger;
			_productRepository=productRepository;
			_unitOfWork=unitOfWork;
		}
		public async Task<CartResultDto> GetMyCartAsync(Guid userId, CancellationToken cancellationToken)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("UserId cannot be empty.", nameof(userId));
			}

			var query = _cartRepository.GetCartQuery(userId);

			CartResultDto? cart = await query.Select(c => new CartResultDto
			{
				CartId = c.Id,
			    TotalItemsCount= c.CartItems.Select(ci=>ci.Quantity).DefaultIfEmpty(0).Sum(),
			    SubTotal= c.CartItems.Select(ci => ci.Quantity * ci.Product.Price.Amount).DefaultIfEmpty(0m).Sum(),
			    HasUnavailableItems= c.CartItems.Any(ci=>ci.Product.IsDeleted||!ci.Product.IsAvailable|| ci.Product.StockQuantity < ci.Quantity),
				CartItems= c.CartItems.Select(ci=>new CartItemDto
				{
					 ProductId=ci.ProductId,
					 Quantity=ci.Quantity,
					 ProductName=ci.Product.Name,
					 UnitPrice= ci.Product.Price.Amount,
					 Total= ci.Quantity* ci.Product.Price.Amount,
					 AvailableStock =(!ci.Product.IsDeleted && ci.Product.IsAvailable)? ci.Product.StockQuantity: 0,
					 IsAvailable = (ci.Product.IsAvailable&& !ci.Product.IsDeleted && ci.Product.StockQuantity>0 && ci.Product.StockQuantity >= ci.Quantity)
				}).ToList()			
			}).FirstOrDefaultAsync(cancellationToken);

			if (cart==null)
			{
				return new CartResultDto();
			}

			return cart;
		}

		public async Task<CartResultDto> AddItemToCartAsync(Guid userId , RequestAddToCartDto dto, CancellationToken cancellationToken)
		{
			if (userId == Guid.Empty)
				throw new ArgumentException("Invalid userId.", nameof(userId));
			if (dto.Quantity <= 0)
				throw new ArgumentException("Quantity must be greater than zero.");
			Product? product =await _productRepository.GetProductByIdAsync(dto.ProductId, cancellationToken);

			if (product is null)
				throw new NotFoundException("Product not found.");
			if (product.IsDeleted || !product.IsAvailable)
				throw new InvalidOperationException("Product is not available.");

			Cart? cart = await _cartRepository.GetMyCartAsync(userId, cancellationToken);
			
			if (cart is null)
			{
				cart = new Cart(userId);
				await _cartRepository.AddCartAsync(cart, cancellationToken);
			}
			var existingQuantity= cart.CartItems.Where(x=>x.ProductId==product.Id).Select(x=>x.Quantity).FirstOrDefault();

			if (existingQuantity+dto.Quantity > product.StockQuantity)
			{
				_logger.LogWarning(	"User {UserId} attempted to exceed stock for product {ProductId}. Requested: {Requested}, Available: {Available}",
					userId,	product.Id,	dto.Quantity,	product.StockQuantity);
				throw new InvalidOperationException("Requested quantity exceeds available stock.");
			}


			cart.AddItem(product.Id, dto.Quantity);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return await GetMyCartAsync(userId, cancellationToken);
		}

		public async Task<CartResultDto> RemoveItemFromCartAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("Invalid userId.", nameof(userId));
			}
			if (productId==Guid.Empty)
			{
				throw new ArgumentException(nameof(productId));
			}

			var cart = await _cartRepository.GetMyCartAsync(userId, cancellationToken);
			if (cart is null)
			{
				return new CartResultDto();
			}
			cart.RemoveItem(productId);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return await GetMyCartAsync(userId, cancellationToken);

		}

		public async Task<CartResultDto> UpdateCartItemQuantityAsync(Guid userId, Guid productId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken)
		{
			if (userId == Guid.Empty)
			{
				throw new ArgumentException("Invalid userId.", nameof(userId));
			}
			if (productId == Guid.Empty)
			{
				throw new ArgumentException(nameof(productId));
			}
			if (dto.Quantity < 0)
			{
				throw new ArgumentException(nameof(dto.Quantity));
			}
			var product = await _productRepository.GetProductByIdAsync(productId, cancellationToken);
			if (product is null)
				throw new NotFoundException("Product not found.");
			if (product.IsDeleted || !product.IsAvailable)
				throw new InvalidOperationException("Product is not available.");
			if (dto.Quantity > product.StockQuantity)
			{
				_logger.LogWarning(
				"User {UserId} attempted to update quantity exceeding stock for product {ProductId}. Requested: {Requested}, Available: {Available}",
				userId,	productId,	dto.Quantity,	product.StockQuantity);

				throw new InvalidOperationException("Requested quantity exceeds available stock.");
			}
			var cart = await _cartRepository.GetMyCartAsync(userId, cancellationToken);
			if (cart is null)
			{
				throw new NotFoundException("Cart not found.");
			}

			cart.UpdateQuantity(productId, dto.Quantity);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			return await GetMyCartAsync(userId, cancellationToken);

		}

		public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("Invalid userId.", nameof(userId));
			}
			var cart = await _cartRepository.GetMyCartAsync(userId, cancellationToken);
			if (cart is null)
			{
				return;
			}
			cart.ClearCart();
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
