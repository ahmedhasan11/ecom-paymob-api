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
		private readonly Lazy<ConnectionMultiplexer> _connection;
		public RedisConnectionFactory(IConfiguration configuration)
		{
			var host = configuration["Redis:Host"];
			var port = configuration["Redis:Port"];
			var password = configuration["Redis:Password"];

			var options = new ConfigurationOptions()
			{
				EndPoints = { $"{host}:{port}" },
				User = "default",
				Password = password,
				Ssl = true,
				AbortOnConnectFail =false
			};
			_connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
		}

		public IDatabase GetDatabase() 
		{
			return _connection.Value.GetDatabase();
		}
	}
}
