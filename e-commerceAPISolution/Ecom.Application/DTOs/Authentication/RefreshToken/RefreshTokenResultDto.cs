using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication.RefreshToken
{
	public class RefreshTokenResultDto
	{
		public string RawToken { get; set; } = default!;
		public string HashedToken { get; set; } = default!;

		public DateTime ExpiresAt { get; set; }
	}
}
