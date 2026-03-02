using Ecom.Application.DTOs.Cart;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Cart
{
	public class UpdateCartItemQuantityDtoValidator:AbstractValidator<UpdateCartItemQuantityDto>
	{
		public UpdateCartItemQuantityDtoValidator()
		{
			RuleFor(x => x.Quantity).GreaterThan(0);
		}
	}
}
