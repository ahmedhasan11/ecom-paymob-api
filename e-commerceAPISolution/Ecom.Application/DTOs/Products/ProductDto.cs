using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Products
{
	public class ProductDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public DateTime CreatedAt { get; set; }
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsInStock { get; set; }
		public string? ImageUrl { get; set; }

		public string? Description { get; set; }

	}
}
