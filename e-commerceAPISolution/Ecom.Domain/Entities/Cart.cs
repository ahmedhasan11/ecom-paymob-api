using Ecom.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecom.Domain.Entities
{
	public class Cart:AuditableEntity
	{
		public Guid Id { get; private set; }= Guid.NewGuid();
		public Guid UserId { get; private set; }

		public List<CartItem> CartItems { get; private set; } = new List<CartItem>();

		private Cart() { }

		
		public Cart(Guid userId)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("UserId cannot be empty.", nameof(UserId));
			}
			UserId = userId;
		}

		public void AddItem(Guid productId, int Quantity)
		{
			if (productId==Guid.Empty)
			{
				throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
			}
			if (Quantity<=0)
			{
				throw new ArgumentException("Quantity must be greater than zero.", nameof(Quantity));
			}
			var existingItem = CartItems.FirstOrDefault(x => x.ProductId == productId);
			if (existingItem is not null)
			{
				existingItem.IncreaseQuantity(Quantity);
				return;
			}
			CartItem cartItem= new CartItem(productId, Quantity);
			CartItems.Add(cartItem);
		}

		public void RemoveItem(Guid productId) 
		{
			if (productId == Guid.Empty)
			{
				throw new ArgumentException("ProductId cannot be empty.", nameof(productId));
			}
			var expectedItem = CartItems.FirstOrDefault(x => x.ProductId == productId);
			if (expectedItem is null)
			{
				return;
			}
			CartItems.Remove(expectedItem);
		}


	}
}
