using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IPaymentRepository
	{
		Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);

		Task<Payment?> GetPaymentByPaymentId(Guid paymentId, CancellationToken cancellationToken);

		Task<Payment?> GetPendingPaymentByOrderId(Guid orderId, CancellationToken cancellationToken);

		Task<Payment?> GetPaymentByPaymobOrderIdAsync(long paymobOrderId, CancellationToken cancellationToken);




	}
}
