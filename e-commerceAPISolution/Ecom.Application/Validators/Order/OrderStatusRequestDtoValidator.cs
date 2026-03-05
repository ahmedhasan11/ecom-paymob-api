using Ecom.Application.DTOs.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Order
{
	public class OrderStatusRequestDtoValidator:AbstractValidator<OrderStatusRequestDto>
	{
		public OrderStatusRequestDtoValidator() 
		{
			RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid order status value");
		}
	}
}
