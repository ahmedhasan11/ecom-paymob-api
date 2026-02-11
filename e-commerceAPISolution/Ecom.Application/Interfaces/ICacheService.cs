using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface ICacheService
	{
		public Task<string?> GetAsync(string key);

		public Task SetAsync(string key, string value, TimeSpan expiry);

		public Task RemoveAsync(string key);
	}
}
