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
			builder.Property(x => x.CheckoutUrl).HasMaxLength(1000).IsRequired(false);
			builder.Property(x => x.ExpiresAt).IsRequired(false);
			builder.Property(x => x.PaidAt).IsRequired(false);
			builder.Property(x => x.FailedAt).IsRequired(false);
			builder.Property(x => x.PaymobOrderId).IsRequired(false);
			builder.Property(x => x.PaymobTransactionId).IsRequired(false);
			builder.HasIndex(x => x.PaymobOrderId).IsUnique().HasFilter("[PaymobOrderId] IS NOT NULL");
			builder.HasIndex(x => x.PaymobTransactionId).IsUnique()	.HasFilter("[PaymobTransactionId] IS NOT NULL");
			builder.HasIndex(x => x.ExpiresAt); //for the background jobs
			builder.HasIndex(x => x.OrderId);
			builder.HasIndex(x => x.Status);
		}
	}
}
