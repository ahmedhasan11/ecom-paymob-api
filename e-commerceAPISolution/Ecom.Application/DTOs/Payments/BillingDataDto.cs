using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Payments
{
	public class BillingDataDto
	{
		public string FirstName { get; set; } = default!;
		public string LastName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string PhoneNumber { get; set; } = default!;

		public string City { get; set; } = default!;
		public string Street { get; set; } = default!;
		public string Country { get; set; } = "EG";
	}
}
