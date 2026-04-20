using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Payments
{
	public class CreatePaymentSessionRequest
	{
		public decimal Amount { get; set; }
		public string Currency { get; set; } = default!;
		public Guid PaymentId { get; set; }

		public Guid OrderId { get; set; }
		public List<CreatePaymentItem> Items { get; set; } = [];

		public BillingDataDto BillingData { get; set; } = default!;

	}
}
