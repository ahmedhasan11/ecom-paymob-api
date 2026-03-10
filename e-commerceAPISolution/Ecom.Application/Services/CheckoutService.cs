using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
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
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReservationRepository _reservationRepository;
		private readonly IProductRepository _productRepository;
		private readonly ILogger<CheckoutService> _logger;
		public CheckoutService(ICartService cartService, IOrderService orderService, IUnitOfWork unitOfWork, ILogger<CheckoutService> logger, IReservationRepository reservationRepository, IProductRepository productRepository) 
		{
			_cartService = cartService;
			_orderService = orderService;
			_unitOfWork = unitOfWork;
			_logger = logger;
			_reservationRepository = reservationRepository;
			_productRepository = productRepository;
		}
		public async Task CheckoutAsync(Guid userId)
		{
			if (userId == Guid.Empty)
				throw new ArgumentException("userId cannot be empty.", nameof(userId));
		}
	}
}
