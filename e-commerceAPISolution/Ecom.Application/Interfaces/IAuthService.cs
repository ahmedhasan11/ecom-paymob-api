using Ecom.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
		Task<AuthResponseDto> LoginAsync(LoginDto dto);
		Task<AuthResponseDto> RefreshSessionAsync(string refreshToken);
		Task<bool> LogoutDeviceAsync(string refreshToken);
		Task<bool> LogoutAllDevicesAsync(Guid userId);

		Task<bool> ChangePasswordAsync(Guid userId,ChangePasswordDto dto);
		Task ForgotPasswordAsync(string email);
		Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto dto);

		Task SendEmailConfirmationAsync( string email);
		Task<bool> ConfirmEmailAsync(ConfirmEmailDto dto);
	}
}
