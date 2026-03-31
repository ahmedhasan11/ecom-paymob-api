using Ecom.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IPaymentService
	{
		Task<PaymentSessionResponse> CreatePaymentSessionAsync(Guid orderId, CancellationToken cancellationToken);
	}
}
