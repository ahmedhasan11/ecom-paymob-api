using Ecom.Domain.Common;
using Ecom.Domain.Exceptions;
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
		public Guid Id { get; private set; }
		public Guid UserId { get; private set; }

		public List<CartItem> CartItems { get; private set; } = new List<CartItem>();

		private Cart() { }

		
		public Cart(Guid userId)
		{
			if (userId==Guid.Empty)
			{
				throw new InputValidationException("UserId cannot be empty.");
			}
			UserId = userId;
		}

		public void AddItem(Guid productId, int Quantity)
		{
			if (productId==Guid.Empty)
			{
				throw new InputValidationException("ProductId cannot be empty.");
			}
			if (Quantity<=0)
			{
				throw new InputValidationException("Quantity must be greater than zero.");
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
				throw new InputValidationException("ProductId cannot be empty.");
			}
			var expectedItem = CartItems.FirstOrDefault(x => x.ProductId == productId);
			if (expectedItem is null)
			{
				return;
			}
			CartItems.Remove(expectedItem);
		}

		public void UpdateQuantity(Guid productId, int newQuantity)
		{
			if (productId == Guid.Empty)
				throw new InputValidationException(nameof(productId));
			var existingItem = CartItems.FirstOrDefault(x=>x.ProductId==productId);
			if (existingItem is null) 
			{
				throw new BusinessException("Cart item not found.");
			}
			if (newQuantity == 0)
			{
				CartItems.Remove(existingItem);
				return;
			}
			existingItem.SetQuantity(newQuantity);

		}

		public void ClearCart()
		{
			CartItems.Clear();
		}


	}
}
