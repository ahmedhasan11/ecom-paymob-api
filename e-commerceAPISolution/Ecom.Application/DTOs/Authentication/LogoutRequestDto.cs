using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class LogoutRequestDto
	{
		public string refreshToken { get; set; } = default!;
	}
}
