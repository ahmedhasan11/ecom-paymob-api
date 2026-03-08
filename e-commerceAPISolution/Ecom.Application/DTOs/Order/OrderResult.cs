using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Order
{
	public class OrderResult
	{
		public Guid OrderId { get; set; }
		public OrderStatusEnum Status { get; set; }
		public decimal Total { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
