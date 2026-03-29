using Ecom.Application.Common.Settings;
using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymentGateway : IPaymentGateway
	{
		private readonly PaymobSettings _paymob;
		
		public PaymentGateway(IOptions<PaymobSettings> paymob) 
		{
			_paymob = paymob.Value;
		}
		public async Task<PaymentSessionResult> CreatePaymentSessionAsync(CreatePaymentSessionRequest req,  CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
