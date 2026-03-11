using Ecom.Application.DTOs.Cart;
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
		public async Task<Guid> CheckoutAsync(Guid userId, CancellationToken cancellationToken, ShippingAddress address)
		{
			if (userId == Guid.Empty)
				throw new ArgumentException("userId cannot be empty.", nameof(userId));
			var pendingOrder = await _orderRepository.GetPendingOrderForUser(userId, cancellationToken);
			if(pendingOrder is not null) 
				return pendingOrder.Id;
			#region Load Cart
			var cart = await _cartService.GetMyCartAsync(userId, cancellationToken);
			#endregion
			#region Validate Cart
			if (cart.TotalItemsCount == 0)
			{
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
					throw new NotFoundException("product not found");
				}
				if (product.IsDeleted || !product.IsAvailable)
				{
					throw new InvalidOperationException("product is unavilable right now ");
				}
				//var reservedQuantity = activeReservations[product.Id]; //momkn trmy exception lw mfe4 reserve l el item
				var reservedQuantity = activeReservations.TryGetValue(product.Id, out var value) ? value : 0 ;
				var AvailableStock = product.StockQuantity - reservedQuantity;

				var quantityRequested = cartItem.Quantity;

				if (quantityRequested > AvailableStock)
				{
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
			Order order = Order.Create(userId,address,requestedItems);
			await _orderRepository.AddOrderAsync(order, cancellationToken);
			#endregion
			#region Create Reservation
			foreach (var cartItem in cart.CartItems)
			{
				InventoryReservation reservation = new InventoryReservation(order.Id, cartItem.ProductId, cartItem.Quantity);
				await _reservationRepository.CreateReservationAsync(reservation, cancellationToken);
			}
			#endregion
			#region Clear Cart
			await _cartService.ClearCartAsync(userId, cancellationToken);
			#endregion
			#region Save
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			#endregion
			return order.Id;
		}

	}
}
