using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class OrderItem
	{
		public Guid Id { get; private set; }
		public Guid OrderId { get; private set; }
		public Order Order { get; private set; }

		public Guid ProductId { get; private set; }

		public string ProductName { get; private set; }

		public decimal UnitPrice { get;	private set; }

		public decimal LineTotal { get; private set; } 

		public int Quantity { get; private set; }



		private OrderItem() { }

		public OrderItem(CreateOrderItemData item)
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
			if (item.Quantity <= 0)
			{
				throw new ArgumentException(nameof(item.Quantity));
			}
			Id = Guid.NewGuid();
			ProductId = item.ProductId;
			ProductName= item.ProductName;
			UnitPrice= item.UnitPrice;
			Quantity= item.Quantity;
			LineTotal = item.UnitPrice * item.Quantity;
		}

		
	}
}
