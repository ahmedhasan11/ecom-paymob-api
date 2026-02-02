using Ecom.Domain.Common;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Product:AuditableEntity
	{


		public Guid Id { get; set; } = Guid.NewGuid();

		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public Money Price { get; set; } = null!;

		public string? ImageUrl { get; set; }

		public Guid? CategoryId { get; set; }
		public Category? Category { get; set; }

		public bool IsAvailable { get; set; } = true;

		public bool IsDeleted { get; set; } = false;
		private Product() { } // For EF Core
		public Product(Decimal price , string name)
		{
			if (string.IsNullOrWhiteSpace(name) /*|| price==null*/)
			{
				throw new ArgumentException("Name cannot be empty.", nameof(name));
			}
			Name=name;
			Price = Money.From(price); // valdiation of price is done already inside Money VO
		}
	}
}
