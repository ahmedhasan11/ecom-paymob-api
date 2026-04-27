using Ecom.Application.DTOs.Payments;
using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
		private readonly IProductRepository _productRepository;
		private readonly IEmailService _emailService;
		private readonly IServiceProvider _serviceProvider;
		private readonly IUnitOfWork _unitOfWork;
		public PaymentWebhookService(IPaymobHmacValidator paymobHmacValidator, ILogger<PaymentWebhookService> logger,
			IPaymentRepository paymentRepository, IOrderRepository orderRepository
			, IReservationRepository reservationRepository, IUnitOfWork unitOfWork, IProductRepository productRepository
			,IEmailService emailService, IServiceProvider serviceProvider)
		{
			_paymobHmacValidator = paymobHmacValidator;
			_logger = logger;
			_paymentRepository = paymentRepository;
			_orderRepository = orderRepository;
			_reservationRepository = reservationRepository;
			_unitOfWork = unitOfWork;
			_productRepository = productRepository;
			_emailService = emailService;
			_serviceProvider = serviceProvider;
		}
		public async Task HandleWebhookAsync(PaymentWebhookRequest request, string receivedHmac, CancellationToken cancellationToken)
		{
			#region 1- Validate request.Obj  
			if (request.Obj == null)
			{
				_logger.LogWarning("Webhook request object is null");
				return;
			}

			#endregion
			#region 2- Validate Hmac
			//validate hmac before doing any webhooc things 
			//call your method which validating hmac and pass to it the recieved and chekc the result
			if (_paymobHmacValidator.IsValid(request.Obj, receivedHmac) == false)
			{
				_logger.LogWarning("Invalid HMAC for payment webhook. Received HMAC: {ReceivedHmac}", receivedHmac);
				throw new InvalidHmacException("Invalid HMAC"); // Consider using a more specific exception type for better error handling
			}
			#endregion
			#region 3- Validate request.Obj.Order && request.Obj.Order.Id
			if (request.Obj.Order == null || request.Obj.Order.Id <= 0)
			{
				_logger.LogWarning("Webhook received with missing or invalid order id");
				return;
			}
			#endregion
			#region 4-  Check Pending attribute
			if (request.Obj.Pending == true)
			{
				_logger.LogInformation("Payment is pending for transaction ID: {TransactionId}", request.Obj.TransactionId);
				return;
			}
			#endregion
			#region 5- GetPayment & validate if not found
			//GetPayment & validate if not found
			var payment = await _paymentRepository.GetPaymentByPaymobOrderIdAsync(request.Obj.Order.Id, cancellationToken);
			if (payment == null)
			{
				_logger.LogWarning("Payment not found for Paymob Order ID: {PaymobOrderId}", request.Obj.Order.Id);
				return;
			}
			#endregion
			#region 6- Idempotency Check if payment is not pending
			if (payment.Status != PaymentStatusEnum.Pending)
			{
				_logger.LogInformation("Payment with Paymob Order ID: {PaymobOrderId} has already been processed with status: {Status}", request.Obj.Order.Id, payment.Status);
				return;
			}
			#endregion
			#region 7-  Get Order & validate if not found
			var order = await _orderRepository.GetOrderByIdAsync(payment.OrderId, cancellationToken);
			if (order == null)
			{
				_logger.LogWarning("Order not found for Order ID: {OrderId} associated with Paymob Order ID: {PaymobOrderId}", payment.OrderId, request.Obj.Order.Id);
				return;
			}
			#endregion
			#region 8- Get Reservation bool && Reservations List
			bool isActiveReservations = await _reservationRepository.HasActiveReservationsAsync(order.Id, cancellationToken);
			List<InventoryReservation> activeReservations = await _reservationRepository.GetActiveReservationsByOrderId(order.Id, cancellationToken);
			#endregion
			#region 9- Success Flow 
			if (request.Obj.Success == true)
			{
				payment.MarkAsSucceeded(request.Obj.TransactionId);
				_logger.LogInformation("Payment marked as succeeded for Paymob Order ID: {PaymobOrderId}, Transaction ID: {TransactionId}", request.Obj.Order.Id, request.Obj.TransactionId);
				if (isActiveReservations == false)
				{
					order.Cancel(true); //refund
					_logger.LogInformation("Order {OrderId} marked as cancelled and requires refund", order.Id);
					_logger.LogInformation("Refund process should be initiated for Order ID: {OrderId} due to successful payment but no active reservations.", order.Id);
					await _unitOfWork.SaveChangesAsync(cancellationToken);
					return;
				}
				var productIds = activeReservations.Select(r => r.ProductId).ToList();
				var products = await _productRepository.GetProductsInBulkAsync(productIds, cancellationToken);
				var productsDict = products.ToDictionary(p => p.Id);

					foreach (var reservation in activeReservations)
					{
						if (!productsDict.TryGetValue(reservation.ProductId, out var product) || product.IsDeleted || !product.IsAvailable)
						{
							_logger.LogWarning("Product with ID {ProductId} not found or not available  while confirming reservation {ReservationId}",
								reservation.ProductId, reservation.Id);

							//throw new InvalidOperationException("Product not found during payment confirmation");
							order.Cancel(true); //refund
							foreach (var res in activeReservations)
							{
								res.Release();
							}
							await _unitOfWork.SaveChangesAsync(cancellationToken);
							return ;
						}
					} //check if any product invalid  

					foreach (var reservation in activeReservations)
					{
					var product= productsDict[reservation.ProductId];
					_logger.LogInformation("Product with ID: {ProductId} has current stock quantity: {StockQuantity} before confirming reservation.", product.Id, product.StockQuantity);
					product.DecreaseStock(reservation.Quantity);

					reservation.Confirm();
					_logger.LogInformation("Reservation with ID: {ReservationId} for Product ID: {ProductId} has been confirmed.", reservation.Id, reservation.ProductId);

					_logger.LogInformation("Product with ID: {ProductId} stock quantity decreased by {Quantity}. New stock quantity: {StockQuantity}.", product.Id, reservation.Quantity, product.StockQuantity);
					} //final execution

					_logger.LogInformation("Reservations for Order ID: {OrderId} have been confirmed.", order.Id);
					order.MarkAsPaid();
					_logger.LogInformation("Order with ID: {OrderId} has been marked as paid.", order.Id);

				try
				{
					await _unitOfWork.SaveChangesAsync(cancellationToken);
				}
				catch(DbUpdateConcurrencyException ex)
				{
					_logger.LogWarning(ex, "Concurrency conflict occurred while saving changes for Order ID: {OrderId} during payment confirmation.  Another webhook likely processed stock first.", order.Id);
					// Handle concurrency conflict, e.g., by retrying, compensating, or logging detailed information for investigation.
					using var scope = _serviceProvider.CreateScope();
					var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

					var dbOrder = await orderRepo.GetOrderByIdAsync(order.Id, cancellationToken);

					if (dbOrder==null)
					{
						_logger.LogError("Order with ID: {OrderId} not found in database during concurrency conflict resolution.", order.Id);
						await _emailService.SendEmailAsync(
						"ahmedelbadawy89910@gmail.com",
						"❌ CRITICAL: Order Missing",
						$"Order {order.Id} was not found during concurrency handling. Manual investigation required.",
						cancellationToken);
						return;
					}
					dbOrder.Cancel(true); // requires refund

					await unitOfWork.SaveChangesAsync(cancellationToken);
					await _emailService.SendEmailAsync(
						"ahmedelbadawy89910@gmail.com",
						"⚠️ Concurrency Conflict",
						$"Order {order.Id} marked for refund due to stock conflict OR needs manual refund.",
						cancellationToken);
					return;
				}
				return;
			}
			#endregion
			#region 10- Failure Flow		
			if (request.Obj.Success == false)
			{
				payment.MarkAsFailed(request.Obj.TransactionId);
				_logger.LogInformation("Payment marked as failed for Paymob Order ID: {PaymobOrderId}, Transaction ID: {TransactionId}", request.Obj.Order.Id, request.Obj.TransactionId);
				if (isActiveReservations == true)
				{
					//Release all Reservations
					foreach (var reservation in activeReservations)
					{
						reservation.Release();
						_logger.LogInformation("Reservation with ID: {ReservationId} for Product ID: {ProductId} has been released due to payment failure.", reservation.Id, reservation.ProductId);
					}
					_logger.LogInformation("Releasing active reservations for Order ID: {OrderId} due to payment failure.", order.Id);
				}
				order.MarkAsPaymentFailed();
				_logger.LogInformation("Order with ID: {OrderId} has been marked as payment failed.", order.Id);
				await _unitOfWork.SaveChangesAsync(cancellationToken);
				return;
			}
			#endregion
		}
	}
}
