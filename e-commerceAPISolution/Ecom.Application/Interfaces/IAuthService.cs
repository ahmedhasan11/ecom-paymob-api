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
		Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken );
		Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
		Task<AuthResponseDto> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken);
		Task<bool> LogoutDeviceAsync(string refreshToken, CancellationToken cancellationToken);
		Task<bool> LogoutAllDevicesAsync(Guid userId, CancellationToken cancellationToken);

		Task<bool> ChangePasswordAsync(Guid userId,ChangePasswordDto dto, CancellationToken cancellationToken);
		Task ForgotPasswordAsync(string email, CancellationToken cancellationToken);
		Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken);

		Task SendEmailConfirmationAsync( string email, CancellationToken cancellationToken);
		Task<bool> ConfirmEmailAsync(ConfirmEmailDto dto, CancellationToken cancellationToken);
	}
}
