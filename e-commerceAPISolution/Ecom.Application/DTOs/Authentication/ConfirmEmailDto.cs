using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class ConfirmEmailDto
	{
		public string Email { get; set; } = default!;
		public string ConfirmationToken { get; set; } = default!;
	}
}
