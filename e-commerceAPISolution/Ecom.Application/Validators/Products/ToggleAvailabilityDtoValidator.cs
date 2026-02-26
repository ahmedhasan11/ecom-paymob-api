using Ecom.Application.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Products
{
	public class ToggleAvailabilityDtoValidator:AbstractValidator<ToggleAvailabilityDto>
	{
		public ToggleAvailabilityDtoValidator() 
		{
			RuleFor(x => x.Id).NotEmpty();
		}
	}
}
