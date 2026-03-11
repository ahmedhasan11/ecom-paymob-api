using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Configurations
{
	public class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
	{
		public void Configure(EntityTypeBuilder<InventoryReservation> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Quantity).IsRequired();
			builder.Property(x=>x.ExpiresAt).IsRequired();

			builder.Property(x => x.Status).HasConversion<string>().IsRequired().HasMaxLength(50);

			builder.HasOne(x => x.Product).WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(x => x.Order).WithMany().HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Cascade);

			builder.HasIndex(x => x.ProductId);
			builder.HasIndex(x => x.OrderId);
			builder.HasIndex(x => x.Status);
			builder.HasIndex(x => new { x.OrderId, x.ProductId }).IsUnique();
		}
	}
}
