using Ecom.Application.DTOs.Payments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Payments
{
	public class CreatePaymentItemValidator:AbstractValidator<CreatePaymentItem>
	{
		public CreatePaymentItemValidator() 
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
			RuleFor(x => x.UnitPrice).GreaterThan(0);
			RuleFor(x=>x.Quantity).GreaterThan(0);
			RuleFor(x=>x.Description).MaximumLength(100).When(x=>x.Description!=null);
	
		}
	}
}
