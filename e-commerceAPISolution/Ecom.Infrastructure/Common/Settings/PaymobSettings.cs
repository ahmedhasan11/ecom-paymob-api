using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Common.Settings
{
	public class PaymobSettings
	{
		public string SecretKey { get; set; } = default!;

		public string PublicKey { get; set; } = default!;

		public int IntegrationId { get; set; }

		public string BaseUrl { get; set; } = default!;

		public string NotificationUrl { get; set; } = default!;

		public string RedirectUrl { get; set; } = default!;

		public int ExpirationSeconds { get; set; }

		public string CheckoutBaseUrl { get; set; } = default!;

		public string HmacSecret { get; set; } = default!;
	}
}
