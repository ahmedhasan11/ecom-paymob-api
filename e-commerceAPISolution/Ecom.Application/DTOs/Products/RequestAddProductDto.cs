using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Products
{
	public class RequestAddProductDto
	{
		[Required]
		
		public string Name { get; set; } = string.Empty;

		[Required]
		public decimal Price { get; set; }

		public string? ImageUrl { get; set; }

		public string? Description { get; set; }
	}
}
