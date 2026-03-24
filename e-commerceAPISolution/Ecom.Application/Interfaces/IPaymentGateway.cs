using Ecom.Application.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IPaymentGateway
	{
		Task<PaymentSessionResult> CreatePaymentSessionAsync(decimal amount, string currency, CancellationToken cancellationToken);
	}
}
