using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Products
{
	public class UpdateStockDto
	{
		public Guid Id { get; set; }

		public int Quantity { get; set; }
	}
}
