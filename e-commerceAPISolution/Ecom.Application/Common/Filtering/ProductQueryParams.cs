using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Common.Filtering
{
	public class ProductQueryParams
	{
		//Paging
		public int pageNumber { get; set; } = 1;
		public int pageSize { get; set; } = 10;

		//Filtering
		public string? search { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
	}
}
