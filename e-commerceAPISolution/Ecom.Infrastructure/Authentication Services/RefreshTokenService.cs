using Ecom.Application.DTOs.Authentication.RefreshToken;
using Ecom.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class RefreshTokenService : IRefreshTokenService
	{
		public Task<RefreshTokenResultDto> GenerateRefreshTokenAsync(DateTime expiresAt)
		{
			throw new NotImplementedException();
		}
	}
}
