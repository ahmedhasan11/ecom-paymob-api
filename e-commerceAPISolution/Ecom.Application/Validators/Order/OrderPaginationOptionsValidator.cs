using Ecom.Application.Common.Pagination;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Order
{
	public class OrderPaginationOptionsValidator:AbstractValidator<OrdersPaginationOptions>
	{
		public OrderPaginationOptionsValidator() 
		{
			RuleFor(x => x.PageNumber).GreaterThan(0);
			RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(50);
		}
	}
}
