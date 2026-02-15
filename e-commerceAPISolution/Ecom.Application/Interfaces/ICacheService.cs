using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface ICacheService
	{
		public Task<T?> GetAsync<T>(string key);

		public Task SetAsync<T>(string key, T value, TimeSpan expiry);

		public Task RemoveAsync(string key);
	}
}
