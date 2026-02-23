using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly AppDbContext _db;
		public RefreshTokenRepository( AppDbContext db) 
		{
		 _db = db;
		}
		public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
		{
			await _db.RefreshTokens.AddAsync(refreshToken);
		}

		public async Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId)
		{
			return await _db.RefreshTokens.Where(token => token.UserId == userId).ToListAsync();
		}

		public async Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken)
		{
			return await _db.RefreshTokens.FirstOrDefaultAsync(token => token.HashedToken == hashedToken);
		}
	}
}
