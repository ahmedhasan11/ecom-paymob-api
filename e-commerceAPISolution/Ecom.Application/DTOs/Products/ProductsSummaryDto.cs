using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Products
{
	public class ProductsSummaryDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsInStock { get; set; }
		public string? ImageUrl { get; set; }

	}
}
