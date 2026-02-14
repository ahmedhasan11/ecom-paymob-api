using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Common.Settings
{
	public class JwtSettings
	{
		public string Issuer { get; set; } = default!;
		public string Audience { get; set; } = default!;

		public string Secret { get; set; } = default!;

		public int ExpiryMinutes { get; set; }
	}
}
