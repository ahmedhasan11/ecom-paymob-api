using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymobItem
	{
		public string name { get; set; } = default!;
		public int amount { get; set; }
		public int quantity { get; set; }

		public string? description { get; set; }
	}
}
