using Ecom.Application.DTOs.Payments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Payments
{
	public class PaymentSessionRequestValidator:AbstractValidator<CreatePaymentSessionRequest>
	{
		public PaymentSessionRequestValidator() 
		{
			RuleFor(x => x.Amount).GreaterThan(0);
			RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
			RuleFor(x=>x.PaymentId).NotEmpty();
			RuleFor(x => x.OrderId).NotEmpty();
			RuleFor(x=>x.Items).NotEmpty();
			RuleForEach(x => x.Items).SetValidator(new CreatePaymentItemValidator());

			RuleFor(x => x.BillingData).NotNull().SetValidator(new BillingDataDtoValidator());

		}
	}
}
