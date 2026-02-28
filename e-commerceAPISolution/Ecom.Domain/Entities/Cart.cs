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


	}
}
