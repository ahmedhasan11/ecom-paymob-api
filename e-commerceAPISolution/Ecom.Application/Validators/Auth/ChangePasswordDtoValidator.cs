using Ecom.Application.DTOs.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Auth
{
	public class ChangePasswordDtoValidator:AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordDtoValidator()
		{
			RuleFor(x => x.oldPassword).NotEmpty();
			RuleFor(x => x.newPassword).NotEmpty();

		}
	}
}
