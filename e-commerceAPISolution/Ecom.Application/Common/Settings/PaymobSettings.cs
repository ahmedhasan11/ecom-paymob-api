using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Common.Settings
{
	public class PaymobSettings
	{
		public string SecretKey { get; set; } = default!;

		public string PublicKey { get; set; } = default!;

		public int PaymentMethodId { get; set; }

		public string BaseUrl { get; set; } = default!;
	}
}
