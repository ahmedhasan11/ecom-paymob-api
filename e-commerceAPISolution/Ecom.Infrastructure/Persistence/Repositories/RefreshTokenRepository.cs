using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		public Task AddRefreshTokenAsync(RefreshToken refreshToken)
		{
			throw new NotImplementedException();
		}

		public Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId)
		{
			throw new NotImplementedException();
		}

		public Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken)
		{
			throw new NotImplementedException();
		}
	}
}
