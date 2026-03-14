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
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.HasKey(x => x.Id);
			builder.HasOne(x=>x.Order).WithMany().HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Restrict);
			builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
			builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
			builder.Property(x=>x.Gateway).HasConversion<string>().HasMaxLength(50).IsRequired();
			builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
			builder.Property(x=>x.OrderId).IsRequired();
			builder.HasIndex(x => x.PaymobOrderId);
			builder.HasIndex(x => x.PaymobTransactionId);
			builder.HasIndex(x => x.OrderId);
		}
	}
}
