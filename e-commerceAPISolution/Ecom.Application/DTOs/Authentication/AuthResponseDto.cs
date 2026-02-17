using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class AuthResponseDto
	{
		public bool IsSuccess { get; set; }

		public string? Token { get; set; }

		public string? RefreshToken { get; set; }
		public DateTime? ExpiresAt { get; set; }
		public IEnumerable<string>? Errors { get; set; }
	}
}
