using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Common.Settings
{
	public class EmailSettings
	{
		public string Host { get; set; } = default!;
		public int Port { get; set; }
		public string Username { get; set; } = default!;
		public string Password { get; set; } = default!;

		public bool EnableSsl { get; set; }

	}
}
