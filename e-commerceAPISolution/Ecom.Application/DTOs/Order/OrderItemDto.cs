using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Order
{
	public class OrderItemDto
	{
		public Guid ItemId { get; set; }
		public string ProductName { get; set; } = default!;

		public decimal UnitPrice { get;  set; }

		public decimal LineTotal { get;  set; }

		public int Quantity { get;  set; }
	}
}
