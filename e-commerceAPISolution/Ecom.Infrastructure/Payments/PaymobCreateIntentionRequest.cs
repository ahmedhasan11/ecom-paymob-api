using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	/// <summary>
	/// Responsible of the request of our system to paymob 
	/// </summary>
	public class PaymobCreateIntentionRequest
	{
		public int amount { get; set; }

		public string currency { get; set; } = default!;

		public int[] payment_methods { get; set; } = [];

		public List<PaymobItem> items { get; set; }= new List<PaymobItem>();

		public string merchant_order_id { get; set; } = default!;

		public PaymobBillingData billing_data { get; set; } = default!;

		public string? special_reference { get; set; }

		public int expiration { get; set; }

	}
}
