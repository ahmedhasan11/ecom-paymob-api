using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class RegisterDto
	{
		public string? FullName { get; set; }

		public string? Email { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Password { get; set; }

		public string? ConfirmPassword { get; set; }

	}
}
