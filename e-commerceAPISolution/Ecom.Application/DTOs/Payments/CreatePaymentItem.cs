using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Payments
{
	public class CreatePaymentItem
	{
		public string Name { get; set; } = default!;
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
	}
}
