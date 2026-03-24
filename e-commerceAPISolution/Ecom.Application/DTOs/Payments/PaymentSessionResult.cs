using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Payments
{
	public class PaymentSessionResult
	{
		public long PaymobOrderId { get; set; }

		public string PaymentUrl { get; set; } = default!;
	}
}
