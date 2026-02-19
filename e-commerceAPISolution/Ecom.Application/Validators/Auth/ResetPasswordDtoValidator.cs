using Ecom.Application.DTOs.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Auth
{
	public class ResetPasswordDtoValidator:AbstractValidator<ResetPasswordDto>
	{
		public ResetPasswordDtoValidator() 
		{
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.Token).NotEmpty();
			RuleFor(x=>x.NewPassword).NotEmpty();
		}
		
	}
}
