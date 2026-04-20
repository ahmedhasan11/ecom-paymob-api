using Ecom.Application.DTOs.Cart;
using Ecom.Application.DTOs.Order;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class CheckoutService : ICheckoutService
	{
		private readonly ICartService _cartService;
		private readonly IOrderService _orderService;
		private readonly IOrderRepository _orderRepository;
		private readonly IProductService _productService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReservationRepository _reservationRepository;
		private readonly IProductRepository _productRepository;
		private readonly ILogger<CheckoutService> _logger;
		public CheckoutService(ICartService cartService, IOrderService orderService, IUnitOfWork unitOfWork,
			ILogger<CheckoutService> logger, IReservationRepository reservationRepository, IProductRepository productRepository
			, IProductService productService, IOrderRepository orderRepository) 
		{
			_cartService = cartService;
			_orderService = orderService;
			_unitOfWork = unitOfWork;
			_logger = logger;
			_reservationRepository = reservationRepository;
			_productRepository = productRepository;
			_productService = productService;
			_orderRepository = orderRepository;
		}
		public async Task<Guid> CheckoutAsync(Guid userId, CancellationToken cancellationToken, ShippingAddressDto addressdto)
		{
			_logger.LogInformation("Initiating checkout process for user {UserId}.", userId);
			if (userId == Guid.Empty) { 
				_logger.LogWarning("Checkout failed: userId is empty.");
				throw new ArgumentException("userId cannot be empty.", nameof(userId));
			}
			var pendingOrder = await _orderRepository.GetPendingOrderForUser(userId, cancellationToken);
			if(pendingOrder is not null) { 
				_logger.LogInformation("Checkout skipped: User {UserId} has an existing pending order with ID {OrderId}. Returning existing order ID.", userId, pendingOrder.Id);
				return pendingOrder.Id;
			}
			#region Load Cart
			var cart = await _cartService.GetMyCartAsync(userId, cancellationToken);
			#endregion
			#region Validate Cart
			if (cart.TotalItemsCount == 0)
			{
				_logger.LogWarning("Checkout failed for user {UserId}: cart is empty.", userId);
				throw new InvalidOperationException("cannot create order for an empty cart");
			}
			#endregion
			#region Load Products
			var productsIds =cart.CartItems.Select(ci => ci.ProductId).ToList(); //get all ProductIds
			List<Product> products = await _productRepository.GetProductsInBulkAsync(productsIds, cancellationToken); //get all products in 1 query
			var productsDict = products.ToDictionary(p => p.Id); //dict for hashing instaed of searching in list O(n)
			Dictionary<Guid, int> activeReservations = await _reservationRepository.GetActiveReservedQuantityBulkAsync(productsIds, cancellationToken);//get all reservations in 1 query
			List<CreateOrderItemData> requestedItems = new List<CreateOrderItemData>();
			foreach (var cartItem in cart.CartItems)
			{
				if (!productsDict.TryGetValue(cartItem.ProductId, out var product))
				{
					_logger.LogWarning("Product with ID {ProductId} not found for cart item. User ID: {UserId}", cartItem.ProductId, userId);
					throw new NotFoundException("product not found");
				}
				if (product.IsDeleted || !product.IsAvailable)
				{
					_logger.LogWarning("Product with ID {ProductId} is unavailable (deleted or not available) for cart item. User ID: {UserId}", cartItem.ProductId, userId);
					throw new InvalidOperationException("product is unavilable right now ");
				}
				//var reservedQuantity = activeReservations[product.Id]; //momkn trmy exception lw mfe4 reserve l el item
				var reservedQuantity = activeReservations.TryGetValue(product.Id, out var value) ? value : 0 ;
				var AvailableStock = product.StockQuantity - reservedQuantity;

				var quantityRequested = cartItem.Quantity;

				if (quantityRequested > AvailableStock)
				{
					_logger.LogWarning("Requested quantity {RequestedQuantity} for product ID {ProductId} exceeds available stock {AvailableStock}. User ID: {UserId}", quantityRequested, product.Id, AvailableStock, userId);
					throw new InvalidOperationException("Requested Quantity is more than available stock ");
				}

				CreateOrderItemData cartItemData= new CreateOrderItemData() 
				{
				 ProductId = product.Id,
				 ProductName = product.Name,
				 Quantity = quantityRequested,
				 UnitPrice= product.Price.Amount
				};
				requestedItems.Add(cartItemData);
			}
			#endregion
			#region CreateOrder
			ShippingAddress address = new ShippingAddress( addressdto.RecipientName, addressdto.PhoneNumber,
				addressdto.City, addressdto.Street, addressdto.BuildingNumber, addressdto.PostalCode);
			Order order = Order.Create(userId,address,requestedItems);
			await _orderRepository.AddOrderAsync(order, cancellationToken);
			#endregion
			#region Create Reservation
			foreach (var cartItem in cart.CartItems)
			{
				InventoryReservation reservation = new InventoryReservation(order.Id, cartItem.ProductId, cartItem.Quantity);
				await _reservationRepository.CreateReservationAsync(reservation, cancellationToken);
				_logger.LogDebug("Created inventory reservation for Order ID {OrderId}, Product ID {ProductId}, Quantity {Quantity}. Reservation ID: {ReservationId}", order.Id, cartItem.ProductId, cartItem.Quantity, reservation.Id);
			}
			#endregion
			#region Clear Cart
			await _cartService.ClearCartAsync(userId, cancellationToken);
			_logger.LogInformation("Cleared cart for user {UserId} after successful checkout. Order ID: {OrderId}", userId, order.Id);
			#endregion
			#region Save
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("Saved checkout data to the database for user {UserId}. Order ID: {OrderId}", userId, order.Id);
			#endregion
			_logger.LogInformation("Checkout process completed successfully for user {UserId}. Created Order ID: {OrderId}", userId, order.Id);
			return order.Id;
		}

	}
}
