using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _db;

		public UserRepository(AppDbContext db)
		{
			_db = db;
		}
		public async Task<UserInfoForShiping?> GetUserInfoById(Guid userId, CancellationToken cancellationToken)
		{
			return await _db.Users
				.Where(u => u.Id == userId)
				.Select(u => new UserInfoForShiping { FullName=u.FullName, Email = u.Email })
				.FirstOrDefaultAsync(cancellationToken);
		}
	}
}
