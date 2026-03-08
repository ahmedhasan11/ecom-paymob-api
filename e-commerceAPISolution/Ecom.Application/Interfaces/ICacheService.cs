using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface ICacheService
	{
		Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
		Task SetAsync<T>(string key, T value, TimeSpan expiry,CancellationToken cancellationToken);
		Task RemoveAsync(string key,CancellationToken cancellationToken);
		Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken);
	}
}
