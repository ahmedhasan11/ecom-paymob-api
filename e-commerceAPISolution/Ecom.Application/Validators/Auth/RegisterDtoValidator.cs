using Ecom.Application.DTOs.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Validators.Auth
{
	public class RegisterDtoValidator : AbstractValidator<RegisterDto>
	{
		public RegisterDtoValidator()
		{
			RuleFor(x=>x.FullName)
				.NotEmpty()
				.MinimumLength(3)
				.MaximumLength(50);


			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();


			RuleFor(x => x.PhoneNumber).NotEmpty()
				.Matches(@"^(?:\+20|20|0)?1[0125][0-9]{8}$")
				.WithMessage("Phone number must be a valid Egyptian mobile number.");

			//RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
			//.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			//.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			//.Matches("[0-9]").WithMessage("Password must contain at least one digit.")
			//.Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character."); 

			RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(5).WithMessage("Password must be at least 5 characters.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.");


			RuleFor(x => x.ConfirmPassword)
				.NotEmpty()
				.Equal(x=>x.Password)
				.WithMessage("Passwords do not match");

		}
	}
}
