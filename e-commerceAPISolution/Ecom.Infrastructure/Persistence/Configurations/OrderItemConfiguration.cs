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
	public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.HasKey(oi => oi.Id);
			builder.Property(oi=>oi.ProductId).IsRequired();
			builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(100);
			builder.Property(oi => oi.UnitPrice).IsRequired().HasPrecision(18,2);
			builder.Property(oi => oi.LineTotal).IsRequired().HasPrecision(18,2);
			builder.Property(oi => oi.Quantity).IsRequired();
			builder.HasIndex(oi => oi.OrderId);
			//builder.HasIndex(oi => oi.ProductId);
		}
	}
}
