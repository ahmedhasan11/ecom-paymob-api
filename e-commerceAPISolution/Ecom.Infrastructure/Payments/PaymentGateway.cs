using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymentGateway : IPaymentGateway
	{
		public async Task<PaymentSessionResult> CreatePaymentSessionAsync(CreatePaymentSessionRequest req,  CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
