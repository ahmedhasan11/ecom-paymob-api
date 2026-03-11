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
		Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

		Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken, CancellationToken cancellationToken);

		Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
	}
}
