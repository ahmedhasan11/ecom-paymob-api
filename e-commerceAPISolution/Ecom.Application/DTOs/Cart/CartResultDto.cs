using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Cart
{
	public class CartResultDto
	{
		public Guid? CartId { get; set; }

		public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();

		public int TotalItemsCount { get; set; }

		public decimal SubTotal { get; set; }

		public bool HasUnavailableItems { get; set; }
	}
}
