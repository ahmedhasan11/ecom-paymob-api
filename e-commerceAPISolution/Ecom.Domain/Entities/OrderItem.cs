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

		public OrderItem(Guid productId, string productName , decimal unitPrice , int quantity)
		{
			if (productId==Guid.Empty)
			{
				throw new ArgumentException();
			}
			if (string.IsNullOrEmpty(productName))
			{
				throw new ArgumentException();
			}
			if (unitPrice<=0)
			{
				throw new ArgumentException();
			}
			if (quantity <= 0)
			{
				throw new ArgumentException();
			}
			Id = Guid.NewGuid();
			ProductId = productId;
			ProductName= productName;
			UnitPrice= unitPrice;
			Quantity= quantity;
			LineTotal = unitPrice* quantity;
		}

		
	}
}
