using Ecom.Application.DTOs.Cart;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Cart
{
	public class RequestAddToCartDtoValidator:AbstractValidator<RequestAddToCartDto>
	{
		public RequestAddToCartDtoValidator() 		
		{
			RuleFor(x => x.ProductId).NotEmpty();
			RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0);		
		}
	}
}
