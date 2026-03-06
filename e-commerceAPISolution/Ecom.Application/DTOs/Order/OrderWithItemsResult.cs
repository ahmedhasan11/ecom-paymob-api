using Ecom.Domain.Enums;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Order
{
	public class OrderWithItemsResult
	{
		public Guid OrderId { get; set; }

		public OrderStatusEnum Status { get; set; }

		public decimal TotalPrice { get; set; }

		public ShippingAddressDto Address { get; set; }

		public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

	}
}
