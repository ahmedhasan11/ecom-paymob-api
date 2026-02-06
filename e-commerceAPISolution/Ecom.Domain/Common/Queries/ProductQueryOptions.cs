using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Common.Queries
{
	public class ProductQueryOptions
	{
		public string? search { get; set; }
		public decimal? minPrice { get; set; }
		public decimal? maxPrice{ get; set; }

		public int pageNumber { get; set; }
		public int pageSize { get; set; }
		public string? sortBy { get; set; }
		public string? sortOrder { get; set; }
	}
}
