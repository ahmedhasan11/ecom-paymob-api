using Ecom.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Common.Settings
{
	public class PaymentConfiguration : IPaymentConfiguration
	{
		private readonly PaymobSettings _paymobSettings;
		public PaymentConfiguration(IOptions<PaymobSettings> paymobSettings)
		{
			_paymobSettings = paymobSettings.Value;
		}
		public int ExpirationSeconds => _paymobSettings.ExpirationSeconds;
	}
}
