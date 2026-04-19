using Ecom.Application.DTOs.Payments;
using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Ecom.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class PaymentWebhookService : IPaymentWebhookService
	{
		private readonly IPaymobHmacValidator _paymobHmacValidator;
		private readonly ILogger<PaymentWebhookService> _logger;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IReservationRepository _reservationRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentWebhookService(IPaymobHmacValidator paymobHmacValidator, ILogger<PaymentWebhookService> logger, IPaymentRepository paymentRepository, IOrderRepository orderRepository
			, IReservationRepository reservationRepository, IUnitOfWork unitOfWork)
		{
			_paymobHmacValidator = paymobHmacValidator;
			_logger = logger;
			_paymentRepository = paymentRepository;
			_orderRepository = orderRepository;
			_reservationRepository = reservationRepository;
			_unitOfWork = unitOfWork;
		}
		public async Task HandleWebhookAsync(PaymentWebhookRequest request, string receivedHmac, CancellationToken cancellationToken)
		{

		}
	}
}
