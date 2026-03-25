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
		public string Currency { get; set; }
		public Guid PaymentId { get; set; } 
	}
}
