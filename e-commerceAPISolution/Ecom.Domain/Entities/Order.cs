using Ecom.Domain.Common;
using Ecom.Domain.Enums;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Order : AuditableEntity
	{
		private List<OrderItem> _privateList= new List<OrderItem>();
		public Guid Id { get; private set; }

		public Guid UserId { get; private set; }

		public OrderStatusEnum Status { get; private set; }

		public decimal SubTotal { get; private set; }
		public decimal TotalAmount { get; private set; }
		public IReadOnlyList<OrderItem> Items => _privateList;

		public string Currency { get; private set; }

		public ShippingAddress Address { get; private set; }

		private Order() {}

		public static Order Create(Guid userId , ShippingAddress address , List<CreateOrderItemData> requestedItems)
		{

			if (userId==Guid.Empty)
			{
				throw new ArgumentException(nameof(userId));
			}
			if (requestedItems==null || !requestedItems.Any())
			{
				throw new ArgumentException(nameof(requestedItems));
			}
			if (address is null)
			{
				throw new ArgumentException(nameof(address));
			}
			var order = new Order ()
			{
				Id=Guid.NewGuid(),
				UserId=userId,
				Address=address,
				Currency="EGP",
				Status=OrderStatusEnum.Pending
			};
			foreach (var item in requestedItems)
			{
				if (item.ProductId==Guid.Empty)
				{
					throw new ArgumentException(nameof(item.ProductId));
				}
				if (string.IsNullOrWhiteSpace(item.ProductName))
				{
					throw new ArgumentException(nameof(item.ProductName));
				}
				if (item.UnitPrice<=0)
				{
					throw new ArgumentException(nameof(item.UnitPrice));
				}
				if (item.Quantity<=0)
				{
					throw new ArgumentException(nameof(item.Quantity));
				}
				OrderItem orderItem = new OrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
				order._privateList.Add(orderItem);
				order.SubTotal += orderItem.LineTotal;
			}
			order.TotalAmount = order.SubTotal;
			return order;
		}
		public void MarkAsPaid()
		{
			if (Status == OrderStatusEnum.Pending)
			{
				Status = OrderStatusEnum.Paid;
			}
			else
			{
				throw new InvalidOperationException();
			}

		}
		public void MarkAsPaymentFailed()
		{
			if (Status == OrderStatusEnum.Pending)
			{
				Status = OrderStatusEnum.PaymentFailed;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		public void Cancel()
		{
			if (Status == OrderStatusEnum.Paid)
			{
				throw new InvalidOperationException("Refund not implemented yet");
			}
			else if (Status == OrderStatusEnum.PaymentFailed)
			{
				Status = OrderStatusEnum.Cancelled;
			}
			else
			{
				throw new InvalidOperationException();
			}

		}
	}
}
