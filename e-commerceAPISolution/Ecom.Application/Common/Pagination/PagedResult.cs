using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Common.Pagination
{
	public class PagedResult<T>
	{
		public IReadOnlyList<T> Items { get; set; } = new List<T>();
		public int PageNumber { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; } //the total items from db

		public int TotalPages { get; set; } //the total pages from db
	}
}
