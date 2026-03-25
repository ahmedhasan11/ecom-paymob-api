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
			RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
			RuleFor(x => x.Currency).NotEmpty();
			RuleFor(x=>x.PaymentId).NotEmpty();
		}
	}
}
