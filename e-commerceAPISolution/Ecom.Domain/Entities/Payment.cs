using Ecom.Domain.Common;
using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Payment:AuditableEntity
	{
		public Guid Id { get; private set; }
		public Guid OrderId { get; private set; }
		public Order Order { get; private set; }
		public decimal Amount { get; private set; }
		public string Currency { get; private set; }
		public PaymentStatusEnum Status { get; private set; }
		public GatewayEnum Gateway { get; private set; }
		public long? PaymobOrderId { get; private set; }
		public long? PaymobTransactionId { get; private set; }
		public DateTime? PaidAt { get; private set; }
		public DateTime? FailedAt { get; private set; }

		public string? CheckoutUrl { get; private set; }
		public DateTime? ExpiresAt { get; private set; }
		private Payment() { }

		public static Payment Create(Guid orderId, decimal amount, string currency)
		{
			if (orderId==Guid.Empty)
			{
				throw new ArgumentException("orderId cannot be empty.");
			}
			if (string.IsNullOrWhiteSpace(currency))
			{
				throw new ArgumentException("currency cannot be null");
			}
			if (amount <= 0)
			{
				throw new ArgumentException("amount can't be less than or equal 0");
			}
			var payment = new Payment() 
			{
				Id= Guid.NewGuid(),
				Status= PaymentStatusEnum.Pending,
				OrderId= orderId,
				Amount=amount,
				Currency=currency,
				Gateway=GatewayEnum.Paymob		
			};
			return payment;
		}

		public void SetPaymentSessionData(long paymobOrderId, string checkoutUrl, DateTime expiresAt)
		{
			if (string.IsNullOrWhiteSpace(checkoutUrl))
			{
				throw new ArgumentException("Invalid CheckoutUrl");
			}
			if (expiresAt <= DateTime.UtcNow)
			{
				throw new ArgumentException("Expiration must be in the future");
			}
			if (paymobOrderId <= 0)
			{
				throw new ArgumentException("paymobOrderId is not valid");
			}
			if (CheckoutUrl != null || ExpiresAt.HasValue || PaymobOrderId.HasValue)
			{
				throw new InvalidOperationException("Payment session already initialized");
			}
			if (Status != PaymentStatusEnum.Pending)
			{
				throw new InvalidOperationException("Cannot initialize session for non-pending payment");
			}

			PaymobOrderId = paymobOrderId;
			CheckoutUrl = checkoutUrl;
			ExpiresAt = expiresAt;
		}
		public bool IsExpired()
		{
			if (!ExpiresAt.HasValue)
				return false;

			return DateTime.UtcNow >= ExpiresAt.Value;
		}
		public void MarkAsSucceeded(long transactionId)
		{
			if (transactionId <= 0)
			{
				throw new ArgumentException("paymobTransactionId is not valid");
			}
			if (Status != PaymentStatusEnum.Pending)
			{
				throw new InvalidOperationException();
			}
			Status = PaymentStatusEnum.Succeeded;
			PaidAt = DateTime.UtcNow;
			PaymobTransactionId = transactionId;
		}
		public void MarkAsFailed(long? transactionId)
		{
			if (Status != PaymentStatusEnum.Pending)
			{
				throw new InvalidOperationException();
			}
			Status = PaymentStatusEnum.Failed;
			FailedAt= DateTime.UtcNow;
			PaymobTransactionId= transactionId;
		}

		public void MarkAsCancelled()
		{
			if (Status != PaymentStatusEnum.Pending)
			{
				throw new InvalidOperationException();
			}
			Status = PaymentStatusEnum.Cancelled;
		}

	}
}
