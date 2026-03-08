using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Order
{
	public class ShippingAddressDto
	{
		public string RecipientName { get;  set; }
		public string PhoneNumber { get;  set; }
		public string City { get;  set; }
		public string Street { get;  set; }
		public string? BuildingNumber { get;  set; }
		public string? PostalCode { get;  set; }
	}
}
