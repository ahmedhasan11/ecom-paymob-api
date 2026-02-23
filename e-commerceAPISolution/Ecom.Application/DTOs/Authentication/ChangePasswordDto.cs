using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class ChangePasswordDto
	{
		public string oldPassword { get; set; } = default!;
		public string newPassword { get; set; } = default!;
	}
}
