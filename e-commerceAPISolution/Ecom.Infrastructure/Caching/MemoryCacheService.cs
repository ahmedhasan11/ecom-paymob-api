using Ecom.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Caching
{
	public class MemoryCacheService : ICacheService
	{
		private readonly IMemoryCache _cache;
		public MemoryCacheService(IMemoryCache memoryCache) 
		{
		 _cache = memoryCache;
		}
		public async Task<T?> GetAsync<T>(string key)
		{
			_cache.TryGetValue(key, out T? value);
			return await Task.FromResult(value);
		}

		public async Task RemoveAsync(string key)
		{
			_cache.Remove(key);
			await Task.CompletedTask;
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
		{
			_cache.Set(key, value, expiry);
			await Task.CompletedTask;
		}
	}
}
