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
		Task<RefreshTokenResultDto> GenerateRefreshTokenAsync(DateTime expiresAt);
	}
}
