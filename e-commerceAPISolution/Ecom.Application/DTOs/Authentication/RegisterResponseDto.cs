using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class RegisterResponseDto
	{
		public bool IsSuccess { get; set; }
		public string Message { get; set; } = default!;
	}
}
