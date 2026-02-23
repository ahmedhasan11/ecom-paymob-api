using Ecom.Application.DTOs.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Auth
{
	public class LogoutDtoValidator:AbstractValidator<LogoutRequestDto>
	{
		public LogoutDtoValidator() 
		{
			RuleFor(x=>x.refreshToken).NotEmpty();
		}
	}
}
