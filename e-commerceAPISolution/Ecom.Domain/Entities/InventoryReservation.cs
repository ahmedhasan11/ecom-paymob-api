using Ecom.Domain.Common;
using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class InventoryReservation:AuditableEntity
	{
		public Guid Id { get; private set; }
		public Guid OrderId { get; private set; }
		public Order Order { get; private set; }
		public Guid ProductId { get; private set; }
		public Product Product { get; private set; }

		public ReservationStatusEnum Status { get; private set; }

		public int Quantity { get; private set; }

		public DateTime ExpiresAt { get; private set; }

		public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
		private InventoryReservation() { }
		public InventoryReservation(Guid orderId , Guid productId, int quantity)
		{
			if (orderId==Guid.Empty)
			{
				throw new ArgumentException("orderId cannot be empty.", nameof(orderId));
			}
			if (productId==Guid.Empty)
			{
				throw new ArgumentException("productId cannot be empty.", nameof(productId));
			}
			if (quantity<=0)
			{
				throw new ArgumentException("quantity cannot be 0 or less.", nameof(quantity));
			}
			Id =Guid.NewGuid();
			OrderId=orderId;
			ProductId=productId;
			Quantity=quantity;
			Status = ReservationStatusEnum.Active;
			ExpiresAt = DateTime.UtcNow.AddMinutes(15);
		}
		public void Confirm()
		{
			//payment succeed
			//lazm mn active
			if (Status != ReservationStatusEnum.Active)
				throw new InvalidOperationException("Only active reservations can be confirmed.");

			Status = ReservationStatusEnum.Confirmed;

		}
		public void Release()
		{
			//payment failed || Order Canceled
			//lazm mn active
			if (Status != ReservationStatusEnum.Active)
				throw new InvalidOperationException("Only active reservations can be Released.");
			Status = ReservationStatusEnum.Released;
			
		}
		public void Expire()
		{
			//background detected en time Expired
			//lazm mn active
			if (Status != ReservationStatusEnum.Active)
				throw new InvalidOperationException("Only active reservations can be Expired.");
			Status = ReservationStatusEnum.Expired;

		}




	}
}
