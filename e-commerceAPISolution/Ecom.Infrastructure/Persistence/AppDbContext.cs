using Ecom.Domain.Entities;
using Ecom.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<ApplicationUser , ApplicationRole, Guid>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
		{
		}
		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
			/*يدوّر على أي كلاس بيطبّق إعدادات كيانات (زي ProductConfiguration)
يشغّل ميثود Configure بتاعته تلقائي*/ 
		}
	}
}
