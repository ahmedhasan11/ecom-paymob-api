using Ecom.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IOrderService
	{
		Task<OrderResult> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequestDto dto);
		Task<List<OrderResult>> GetUserOrdersSummaryAsync(Guid userId);
	}
}
