using Ecom.Application.Common.Pagination;
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
		Task<OrderResult> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequestDto dto, CancellationToken cancellationToken);
		Task<PagedResult<OrderResult>> GetUserOrdersSummaryAsync(Guid userId, OrdersPaginationOptions paginationOptions, CancellationToken cancellationToken);
		Task<OrderDetailsResult> GetOrderDetails(Guid userId, Guid orderId, CancellationToken cancellationToken);
	}
}
