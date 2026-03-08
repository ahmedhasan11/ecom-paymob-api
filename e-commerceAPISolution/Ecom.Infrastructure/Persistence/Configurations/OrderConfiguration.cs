using Ecom.Domain.Entities;
using Ecom.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Configurations
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasKey(o => o.Id);
			builder.Property(o => o.UserId).IsRequired();
			builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(o=>o.UserId).OnDelete(DeleteBehavior.Restrict);
			builder.Property(o => o.SubTotal).IsRequired().HasPrecision(18,2);
			builder.Property(o => o.TotalAmount).IsRequired().HasPrecision(18,2); 
			builder.HasMany(o => o.Items).WithOne(oi => oi.Order).HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);
			builder.Property(o => o.Currency).IsRequired().HasMaxLength(10);
			builder.Property(o=>o.Status).HasConversion<string>().IsRequired().HasMaxLength(50);//status order enum
			builder.OwnsOne(o=>o.Address, address =>
			{
				address.Property(a => a.RecipientName).IsRequired().HasMaxLength(100);
				address.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(20);
				address.Property(a => a.City).IsRequired().HasMaxLength(100);
				address.Property(a=>a.Street).IsRequired().HasMaxLength(200);
				address.Property(a => a.BuildingNumber).HasMaxLength(50);
				address.Property(a => a.PostalCode).HasMaxLength(20);
			});
			builder.Navigation(o => o.Address).IsRequired();
			builder.HasIndex(o => o.UserId);

		}
	}
}
