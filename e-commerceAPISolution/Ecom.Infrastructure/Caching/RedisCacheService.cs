
using Ecom.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Caching
{
	public class RedisCacheService : ICacheService
	{
		private readonly IDatabase? _db;
		private readonly IMemoryCache _fallbackCache;
		private readonly bool _redisAvailable;

		public RedisCacheService(RedisConnectionFactory connectionFactory , IMemoryCache memoryCache)
		{
			_fallbackCache = memoryCache;
			try
			{
				_db = connectionFactory.GetDatabase();
				_db.Ping();
				_redisAvailable = true;
			}
			catch
			{
				_redisAvailable = false;
			}
		}

		public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
		{
			if (_redisAvailable)
			{
				try
				{
					var value = await _db!.StringGetAsync(key).WaitAsync(cancellationToken);
					if (!value.HasValue) return default;

					return JsonSerializer.Deserialize<T>(value!);
				}
				catch
				{
					// fallback
				}
			}

			_fallbackCache.TryGetValue(key, out T? cached);
			return cached;
		}

		public async Task SetAsync<T>(string key , T value , TimeSpan expiry, CancellationToken cancellationToken)
		{
			if (_redisAvailable)
			{
				try
				{
					var json = JsonSerializer.Serialize(value);
					await _db!.StringSetAsync(key, json, expiry).WaitAsync(cancellationToken);
					return;
				}
				catch
				{
					// fallback
				}
			}

			_fallbackCache.Set(key, value, expiry);
		}

		public async Task RemoveAsync(string key, CancellationToken cancellationToken)
		{
			if (_redisAvailable)
			{
				try
				{
					await _db!.KeyDeleteAsync(key).WaitAsync(cancellationToken);
					return;
				}
				catch
				{
				}
			}

			_fallbackCache.Remove(key);
		}

		public  async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
		{
			if (_redisAvailable)
			{
				try
				{
					var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
					var keys = server.Keys(pattern: $"{prefix}*").ToArray();

					foreach (var key in keys) 
					{
						await _db.KeyDeleteAsync(key).WaitAsync(cancellationToken);
					}
					return;
				}
				catch
				{
					//falback
				}

			}
		}
	}
}
