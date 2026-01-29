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
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			// ID : Primary Key 
			//Name : Required , MaxLength(100)
			//Price : Required , Decimal , Numeric
			//Description : MaxLength(200)
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
			builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
			builder.Property(p => p.Description).HasMaxLength(200);

		}
	}
}
