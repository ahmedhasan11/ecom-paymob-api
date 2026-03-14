using Ecom.Domain.Common;
using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Payment:AuditableEntity
	{
		public Guid Id { get; private set; }
		public Guid OrderId { get; private set; }
		public Order Order { get; private set; }
		public decimal Amount { get; private set; }
		public string Currency { get; private set; }
		public PaymentStatusEnum Status { get; private set; }
		public GatewayEnum Gateway { get; private set; }
		public long? PaymobOrderId { get; private set; }
		public long? PaymobTransactionId { get; private set; }
		public DateTime? PaidAt { get; private set; }
		public DateTime? FailedAt { get; private set; }
		private Payment() { }



	}
}
