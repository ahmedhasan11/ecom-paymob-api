using Ecom.Application.DTOs.Authentication.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IRefreshTokenService
	{
		RefreshTokenResultDto GenerateRefreshTokenAsync(DateTime expiresAt);
		string HashToken(string rawToken);
	}
}
