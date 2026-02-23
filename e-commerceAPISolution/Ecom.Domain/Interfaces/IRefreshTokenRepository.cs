using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task AddRefreshTokenAsync(RefreshToken refreshToken);

		Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken);

		Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId);
	}
}
