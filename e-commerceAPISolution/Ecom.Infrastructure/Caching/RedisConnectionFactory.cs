using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Caching
{
	public class RedisConnectionFactory
	{
		private readonly ConnectionMultiplexer _connection;
		public RedisConnectionFactory(IConfiguration configuration)
		{
			var connectionString = configuration["Redis:ConnectionString"];
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentException("Redis connection string is not configured.");
			_connection = ConnectionMultiplexer.Connect(connectionString);
		}

		public IDatabase GetDatabase() 
		{
			return _connection.GetDatabase();
		}
	}
}
