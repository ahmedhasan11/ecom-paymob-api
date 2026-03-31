using Ecom.Application.DTOs.Payments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Payments
{
	public class BillingDataDtoValidator:AbstractValidator<BillingDataDto>
	{
		public BillingDataDtoValidator() 
		{
			RuleFor(x => x.FirstName)
				.NotEmpty().MaximumLength(100);

			RuleFor(x => x.LastName)
				.NotEmpty().MaximumLength(100);

			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();

			RuleFor(x => x.PhoneNumber)
				.NotEmpty();

			RuleFor(x => x.City)
			.NotEmpty().MaximumLength(100);

			RuleFor(x => x.Street)
				.NotEmpty().MaximumLength(100);	
		}
	}
}
