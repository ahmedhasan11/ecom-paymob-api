using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.ValueObjects
{
	public class ShippingAddress
	{
		public string RecipientName { get; private set; }
		public string PhoneNumber { get; private set; }
		public string City { get; private set; }
		public string Street { get; private set; }
		public string? BuildingNumber { get; private set; }
		public string? PostalCode { get; private set; }

		private ShippingAddress() { } // For EF

		public ShippingAddress(
			string recipientName,
			string phoneNumber,
			string city,
			string street,
			string? buildingNumber,
			string? postalCode)
		{
			if (string.IsNullOrWhiteSpace(recipientName))
				throw new ArgumentException();

			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new ArgumentException();

			if (string.IsNullOrWhiteSpace(city))
				throw new ArgumentException();

			if (string.IsNullOrWhiteSpace(street))
				throw new ArgumentException();

			RecipientName = recipientName;
			PhoneNumber = phoneNumber;
			City = city;
			Street = street;
			BuildingNumber = buildingNumber;
			PostalCode = postalCode;
		}
	}
}
