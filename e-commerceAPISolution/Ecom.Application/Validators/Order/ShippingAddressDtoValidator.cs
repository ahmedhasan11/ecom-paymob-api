using Ecom.Application.DTOs.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Order
{
	public class ShippingAddressDtoValidator:AbstractValidator<ShippingAddressDto>
	{
		public ShippingAddressDtoValidator() 
		{
			RuleFor(x => x.RecipientName)
				.NotEmpty()
				.MaximumLength(100);

			RuleFor(x => x.PhoneNumber)
				.NotEmpty()
				.Matches(@"^\+?[1-9]\d{7,14}$")
				.WithMessage("Invalid phone number format.")
				.MaximumLength(20);

			RuleFor(x => x.City)
				.NotEmpty()
				.MaximumLength(100);

			RuleFor(x => x.Street)
				.NotEmpty()
				.MaximumLength(200);

			RuleFor(x => x.BuildingNumber)
				.MaximumLength(50)
				.When(x => x.BuildingNumber != null);

			RuleFor(x => x.PostalCode)
				.MaximumLength(20)
				.When(x => x.PostalCode != null);
		}
	}
}
