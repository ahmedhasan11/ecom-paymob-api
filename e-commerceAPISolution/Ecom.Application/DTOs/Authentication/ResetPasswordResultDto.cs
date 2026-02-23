using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class ResetPasswordResultDto
	{
		public bool IsSuccess { get; set; }

		public List<string>? Errors { get; set; }
	}
}
