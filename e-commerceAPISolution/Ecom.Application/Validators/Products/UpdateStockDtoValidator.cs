using Ecom.Application.DTOs.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Products
{
	public class UpdateStockDtoValidator:AbstractValidator<UpdateStockDto>
	{
		public UpdateStockDtoValidator() 
		{
			RuleFor(x=>x.Quantity).NotEmpty().GreaterThan(0);
		}
	}
}
