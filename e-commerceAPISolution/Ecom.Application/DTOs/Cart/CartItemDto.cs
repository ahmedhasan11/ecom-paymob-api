using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Cart
{
	public class CartItemDto
	{
		public Guid ProductId { get; set; }

		public string ProductName { get; set; } = default!;

		public string ImageUrl { get; set; }=default!;

		public decimal UnitPrice { get; set; }

		public int Quantity { get; set; }

		public decimal Total { get; set; }

		public bool IsAvailable { get; set; }
	}
}
