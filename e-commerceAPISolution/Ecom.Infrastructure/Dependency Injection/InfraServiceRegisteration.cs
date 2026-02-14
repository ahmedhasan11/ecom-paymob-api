using Ecom.Application.Common.Settings;
using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using Ecom.Infrastructure.Caching;
using Ecom.Infrastructure.Persistence;
using Ecom.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Dependency_Injection
{
	public static class InfraServiceRegisteration
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
		{
			services.AddDbContext<AppDbContext>
				(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddSingleton<ICacheService, RedisCacheService>();
			services.AddSingleton<RedisConnectionFactory>();

			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			return services;
		}
	}
}
