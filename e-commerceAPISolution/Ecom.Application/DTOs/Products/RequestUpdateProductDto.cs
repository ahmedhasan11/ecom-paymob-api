using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Products
{
	public class RequestUpdateProductDto
	{
		[StringLength(100)]
		public string? Name { get; set; }
		[Range(0, 10000)]
		public decimal? Price { get; set; }

		public string? ImageUrl { get; set; }

		public string? Description { get; set; }
	}
}
