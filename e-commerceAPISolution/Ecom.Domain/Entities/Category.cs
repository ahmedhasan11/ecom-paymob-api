using Ecom.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Entities
{
	public class Category:AuditableEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;

		public string? Slug { get; set; }
		public string? Description { get; set; }

		public string? ImageUrl { get; set; }

		public bool IsActive { get; set; } = true;
		public bool IsDeleted { get; set; } = false;

		public List<Product> Products { get; set; }= new List<Product>();
	}
}
