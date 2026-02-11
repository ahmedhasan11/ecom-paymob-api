
using Ecom.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Caching
{
	public class RedisCacheService : ICacheService
	{
		private readonly IDatabase _db;
		public RedisCacheService(RedisConnectionFactory connectionFactory)
		{
			_db = connectionFactory.GetDatabase();
		}

		public async Task<string?> GetAsync(string key)
		{
			return await _db.StringGetAsync(key);
		}

		public async Task SetAsync(string key , string value , TimeSpan expiry)
		{
			 await _db.StringSetAsync(key, value, expiry);
			//Redis يمسحه تلقائيًا بعد المدة (TTL)
		}

		public async Task RemoveAsync(string key)
		{
			await _db.KeyDeleteAsync(key);
			//دي هنستخدمها لما يحصل Update أو Delete لمنتج
		}
	}
}
