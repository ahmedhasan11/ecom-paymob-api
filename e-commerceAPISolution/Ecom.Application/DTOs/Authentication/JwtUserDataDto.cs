using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Authentication
{
	public class JwtUserDataDto
	{
		public Guid UserId { get; set; }
		public string Email { get; set; } = default!;

		public string FullName { get; set; } = default!;

		public IList<string> Roles { get; set; } = new List<string>();
	}
}
