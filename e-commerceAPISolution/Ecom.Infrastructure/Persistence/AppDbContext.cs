using Ecom.Domain.Common;
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
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		public DbSet<InventoryReservation> InventoryReservations { get; set; }

		public DbSet<Payment> Payments { get; set; }
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries<AuditableEntity>();

			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = DateTime.UtcNow;
				}

				if (entry.State == EntityState.Modified)
				{
					// Only update the timestamp if at least one property (other than CreatedAt/UpdatedAt) has actually changed.
					// This prevents accidental updates (and concurrency errors) on entities like Product that are just being referenced.
					var isActuallyModified = entry.Properties.Any(p => p.IsModified &&
						p.Metadata.Name != nameof(AuditableEntity.CreatedAt) &&
						p.Metadata.Name != nameof(AuditableEntity.UpdatedAt));

					if (isActuallyModified)
					{
						entry.Entity.UpdatedAt = DateTime.UtcNow;
						entry.Property(x => x.CreatedAt).IsModified = false;
					}
					else
					{
						// If nothing changed, tell EF to ignore this entity during the save.
						entry.State = EntityState.Unchanged;
					}
				}
			} // Closing the foreach (var entry in entries) loop
				return await base.SaveChangesAsync(cancellationToken);
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
			/*يدوّر على أي كلاس بيطبّق إعدادات كيانات (زي ProductConfiguration)
يشغّل ميثود Configure بتاعته تلقائي*/ 
		}
	}
}
