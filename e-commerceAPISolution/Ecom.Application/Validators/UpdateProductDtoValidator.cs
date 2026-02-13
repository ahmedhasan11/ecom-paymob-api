using Ecom.Application.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators
{
	public class UpdateProductDtoValidator : AbstractValidator<RequestUpdateProductDto>
	{
		public UpdateProductDtoValidator() 
		{
			RuleFor(x => x.Name).MaximumLength(100).NotEmpty().When(x=>x.Name!=null);
			RuleFor(x => x.Price).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10000).When(x=>x.Price!=null);
			RuleFor(x => x.Description).MaximumLength(500).When(x=>x.Description != null);
		}
	}
}
