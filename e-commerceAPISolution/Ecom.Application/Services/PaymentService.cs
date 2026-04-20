using Ecom.Application.DTOs.Payments;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
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
	public class PaymentService : IPaymentService
	{
		private readonly IPaymentRepository _paymentRepository;
		private readonly IPaymentGateway _paymentGateway;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderRepository _orderRepository;
		private readonly IReservationRepository _reservationRepository;
		private	readonly IPaymentConfiguration _paymentConfiguration;
		private readonly IUserRepository _userRepository;
		private readonly ILogger<PaymentService> _logger;
		public PaymentService(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway, IUnitOfWork unitOfWork, IOrderRepository orderRepository,
			IReservationRepository reservationRepository, IPaymentConfiguration paymentConfiguration, IUserRepository userRepository, ILogger<PaymentService> logger)
		{
			_paymentRepository = paymentRepository;
			_paymentGateway = paymentGateway;
			_unitOfWork = unitOfWork;
			_orderRepository = orderRepository;
			_reservationRepository= reservationRepository;
			_paymentConfiguration= paymentConfiguration;
			_userRepository = userRepository;
			_logger=logger;
		}
		public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(Guid orderId,Guid userId, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Creating payment session for order {OrderId} and user {UserId}.", orderId, userId);
			if (orderId == Guid.Empty)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: orderId is empty.");
				throw new ArgumentException("orderId cannot be empty.",nameof(orderId));
			}
			if (userId==Guid.Empty)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: userId is empty.");
				throw new ArgumentException("userId cannot be empty.",nameof(userId));
			}
			//Load Order
			#region Load Order
			var order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken); 
			#endregion

			//Validate Order : status only accpedted --> 1-Pending , 2-PaymentFailed
			#region Validate Order
			if (order == null)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: Order with ID {OrderId} not found.", orderId);
				throw new NotFoundException("there is no order with this id");
			}
			if (order.Status == OrderStatusEnum.Paid)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: Order with ID {OrderId} is already paid.", orderId);
				throw new InvalidOperationException("cannot make a payment session for a already paid order");
			}
			if (order.Status == OrderStatusEnum.Cancelled)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: Order with ID {OrderId} is cancelled.", orderId);
				throw new InvalidOperationException("cannot make a payment session for a already cancelled order");
			}
			#endregion
			if (order.UserId != userId)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: User {UserId} is not authorized to create payment session for Order {OrderId}.", userId, orderId);
				throw new UnauthorizedAccessException();
			}

			//check if reservations still active(for multiple payments use case)
			#region Check Active Reservations
			bool hasActiveReservation = await _reservationRepository.HasActiveReservationsAsync(orderId, cancellationToken);
			if (!hasActiveReservation)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: No active reservations found for Order {OrderId}.", orderId);
				throw new InvalidOperationException("Reservation expired , please checkout again");
			}
			#endregion



			//check pending payment
			#region Check Pending Payment & Validate its scenarios
			var pendingPayment = await _paymentRepository.GetPendingPaymentByOrderId(orderId, cancellationToken);
			//if there is pending payment , check if its not expired , if not expired --> return checkoutUrl
			if (pendingPayment is not null)
			{
				_logger.LogInformation("Found existing pending payment with ID {PaymentId} for Order {OrderId}. Checking expiration.", pendingPayment.Id, orderId);
				if (!pendingPayment.IsExpired())
				{
					if (string.IsNullOrWhiteSpace(pendingPayment.CheckoutUrl)) 
					{
						_logger.LogWarning("Invalid payment session: Pending payment with ID {PaymentId} has no checkout URL.", pendingPayment.Id);
						throw new InvalidOperationException("Invalid payment session");
					}

					_logger.LogInformation("Pending payment with ID {PaymentId} for Order {OrderId} is still valid. Returning existing checkout URL.", pendingPayment.Id, orderId);
					return new PaymentSessionResponse
					{
						CheckoutUrl = pendingPayment.CheckoutUrl
					};
				}
				pendingPayment.MarkAsFailed(null); //3shan lw payment expired , temp l7d ma n3ml el expired state
				_logger.LogInformation("Pending payment with ID {PaymentId} for Order {OrderId} has expired. Marking as failed.", pendingPayment.Id, orderId);
				await _unitOfWork.SaveChangesAsync(cancellationToken);
				_logger.LogInformation("Expired payment with ID {PaymentId} for Order {OrderId} marked as failed.", pendingPayment.Id, orderId);
			} 
			#endregion



			//take data from user & order.shippingaddress
			var user = await _userRepository.GetUserInfoById(userId, cancellationToken);
			if (user == null)
			{
				_logger.LogWarning("CreatePaymentSessionAsync failed: User with ID {UserId} not found.", userId);
				throw new NotFoundException("User not found ");
			}



			//build billing data 
			var billingData = BuildBillingData(user, order);

			//if payment expired --> create new payment entity
			//if no pending payment --> create new payment entity
			Payment payment = Payment.Create(orderId, order.TotalAmount, order.Currency);
			_logger.LogInformation("Created new payment with ID {PaymentId} for Order {OrderId}.", payment.Id, orderId);


			//add payment record to db 
			//save changes 3shan n2dr nb3t l paymob el merchant == payment.id
			await _paymentRepository.CreatePaymentAsync(payment, cancellationToken);
			_logger.LogInformation("Payment with ID {PaymentId} for Order {OrderId} added to the database.", payment.Id, orderId);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("Payment with ID {PaymentId} for Order {OrderId} saved to the database.", payment.Id, orderId);


			//List<CreatePaymentItem> paymentIems = new List<CreatePaymentItem>();
			var paymentItems= order.Items.Select(oi=>new CreatePaymentItem { Name=oi.ProductName, UnitPrice= oi.UnitPrice, Quantity=oi.Quantity }).ToList();


			CreatePaymentSessionRequest paymentRequest = new CreatePaymentSessionRequest { PaymentId=payment.Id,
				OrderId = payment.OrderId, Amount= payment.Amount, Currency= payment.Currency, Items= paymentItems, BillingData= billingData   };


			//call the payment gateway 
			//get the paymob gateway response 
			// check if payment gateway throws an error --> payment.MarkAsFailed() , dont make the reservation release, save changes , throw exception
			PaymentSessionResult paymobResponse;
			try
			{
				_logger.LogInformation("Calling payment gateway for Payment ID {PaymentId} and Order {OrderId}", payment.Id, orderId);
				paymobResponse = await _paymentGateway.CreatePaymentSessionAsync(paymentRequest, cancellationToken);
			}
			catch(Exception)
			{
				payment.MarkAsFailed(null);
				//mark as failed bs mt3ml4 release , e7ns bs bn3ml kda hna 3shan n2ol en session creatuion baz
				//bs el user y2dr y3ml pay tany tol ma el resertvation active 
				//el release yt3ml f el payment failed webhook bs 
				await _unitOfWork.SaveChangesAsync(cancellationToken);
				_logger.LogInformation("Payment with ID {PaymentId} for Order {OrderId} marked as failed in the database after payment gateway error.", payment.Id, orderId);
				throw; //rethrow
			}


			var expiresAt = DateTime.UtcNow.AddSeconds(_paymentConfiguration.ExpirationSeconds);

			//save paymobData inside payement entity
			payment.SetPaymentSessionData(paymobResponse.PaymobOrderId, paymobResponse.CheckoutUrl, expiresAt);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("Payment session data saved to the database for Payment ID {PaymentId} and Order {OrderId}.", payment.Id, orderId);
			//return checkout url from the payment gateway
			_logger.LogInformation("Payment session created successfully for Payment ID {PaymentId} and Order {OrderId}. Checkout URL returned to the caller.", payment.Id, orderId);
			return new PaymentSessionResponse { CheckoutUrl = paymobResponse.CheckoutUrl };


		}
		private BillingDataDto BuildBillingData(UserInfoForShiping user , Order order)
		{
			var fullName = user.FullName?.Trim();

			var parts = string.IsNullOrWhiteSpace(fullName)
				? Array.Empty<string>()
				: fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			string firstName;
			string lastName;

			if (parts.Length == 0)
			{
				firstName = "User";
				lastName = "User";
			}
			else if (parts.Length == 1)
			{
				firstName = parts[0];
				lastName = "N/A"; // أو "-" أو "User"
			}
			else
			{
				firstName = parts[0];
				lastName = parts[^1];
			}
			return new BillingDataDto
			{
				City = order.Address.City,
				Street = order.Address.Street,
				PhoneNumber = order.Address.PhoneNumber,
				Email = user.Email,
				FirstName = firstName,
				LastName = lastName
			};
		}
	}
}
