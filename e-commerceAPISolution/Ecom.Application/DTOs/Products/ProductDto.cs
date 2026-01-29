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
		public string Name { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; }
		public decimal Price { get; set; }



	}
}
