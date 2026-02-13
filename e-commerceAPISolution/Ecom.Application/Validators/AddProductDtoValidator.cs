using Ecom.Application.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators
{
	public class AddProductDtoValidator : AbstractValidator<RequestAddProductDto>
	{


		public AddProductDtoValidator() 
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
			RuleFor(x => x.Price).NotEmpty().GreaterThanOrEqualTo(0).LessThanOrEqualTo(10000);
			RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null); ;
		}
	}
}
