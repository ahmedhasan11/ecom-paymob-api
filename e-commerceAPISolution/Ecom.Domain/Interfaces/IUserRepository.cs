using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IUserRepository
	{
		Task<UserInfoForShiping?> GetUserInfoById(Guid userId, CancellationToken cancellationToken);
	}
}
