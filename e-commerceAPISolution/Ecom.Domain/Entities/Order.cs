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

		//public Order(Guid userId)
		//{
		//	if (userId==Guid.Empty)
		//	{
		//		throw new ArgumentException();
		//	}
		//	UserId= userId;
		//	Id = Guid.NewGuid();		
		//	Status = OrderStatusEnum.Pending;
		//	TotalAmount = SubTotal;
		//}
		//public void MarkAsPaid()
		//{
		//	if (Status==OrderStatusEnum.Pending)
		//	{
		//		Status = OrderStatusEnum.Paid;
		//	}
		//	else
		//	{
		//		throw new ArgumentException();
		//	}

		//}

		//public void MarkAsPaymentFailed()
		//{
		//	if (Status==OrderStatusEnum.Pending)
		//	{
		//		Status= OrderStatusEnum.PaymentFailed;
		//	}
		//	else
		//	{
		//		throw new ArgumentException();
		//	}
		//}
		//public void Cancel()
		//{
		//	if (Status==OrderStatusEnum.Paid)
		//	{
		//		throw new ArgumentException("Refund not implemented yet");
		//	}
		//	else if (Status==OrderStatusEnum.PaymentFailed)
		//	{
		//		Status = OrderStatusEnum.Cancelled;
		//	}
		//	else
		//	{
		//		throw new ArgumentException();
		//	}

		//}
	}
}
