using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Product
	{
		public Guid Id { get; set; }

		public string Name { get; set; }
		public string? Description { get; set; }
		public Money Price { get; set; }

		public string? ImageUrl { get; set; }

		public Guid? CategoryId { get; set; }

		public bool IsAvailable { get; set; } = true;

		public bool IsDeleted { get; set; } = false;
	}
}
