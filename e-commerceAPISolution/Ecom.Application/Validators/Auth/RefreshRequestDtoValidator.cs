using Ecom.Application.DTOs.Authentication.RefreshToken;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Auth
{
	public class RefreshRequestDtoValidator:AbstractValidator<RefreshRequestDto>
	{
		public RefreshRequestDtoValidator() 
		{
			RuleFor(x => x.RefreshToken).NotEmpty();
		}
	}
}
