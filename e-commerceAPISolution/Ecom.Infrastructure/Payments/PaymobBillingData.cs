using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymobBillingData
	{
		public string first_name { get; set; } = default!;
		public string last_name { get; set; } = default!;
		public string email { get; set; } = default!;
		public string phone_number { get; set; } = default!;

		public string city { get; set; } = default!;
		public string street { get; set; } = default!;

		public string country { get; set; } = "EG";
	}
}
