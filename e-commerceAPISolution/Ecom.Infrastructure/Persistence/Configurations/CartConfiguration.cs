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
	public class CartConfiguration : IEntityTypeConfiguration<Cart>
	{
		public void Configure(EntityTypeBuilder<Cart> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.UserId).IsRequired();

			builder.HasMany(x=>x.CartItems).WithOne(ci=>ci.Cart).HasForeignKey(x=>x.CartId).OnDelete(DeleteBehavior.Cascade);

			builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Cart>(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
		}
	}
}
