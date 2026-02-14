using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class JwtResultDto
	{
		public string Token { get; set; } = default!;

		public DateTime ExpiresAt { get; set; }
	}
}
