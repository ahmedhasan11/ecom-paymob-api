using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class CartItem
	{
		public Guid Id { get; private set; }= Guid.NewGuid();

		public Cart Cart { get; private set; }
		public Guid CartId { get; private set; }


		public Product Product { get; private set; }

		public Guid ProductId { get; private set; }

		public int Quantity { get; private set; }

		public decimal UnitPrice { get; private set; }

		public decimal Total => Quantity * UnitPrice;

		private CartItem() { }

		public CartItem(Guid productId, int quantity, decimal unitPrice)
		{
			ProductId = productId;
			Quantity = quantity;
			UnitPrice = unitPrice;
		}

		public void IncreaseQuantity(int quantity)
		{
			if (quantity<=0)
			{
				throw new ArgumentException("quantity cannot be 0 or less", nameof(quantity));
			}
			Quantity += quantity;
		}

		public void DecreaseQuantity(int quantity) 
		{
			if (quantity <= 0)
			{
				throw new ArgumentException("quantity cannot be 0 or less", nameof(quantity));
			}
			if (Quantity-quantity <= 0)
			{
				throw new ArgumentException("Invalid quantity");
			}
			Quantity -= quantity;
		}
	}
}
