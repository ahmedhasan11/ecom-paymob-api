using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IOrderRepository
	{
		Task AddOrderAsync(Order order);
		Task<Order?> GetOrderByIdAsync(Guid orderId); // for changing order status , you dont need to return OrderItems 

		IQueryable<Order> GetUserOrdersQuery(Guid userId);
	}
}
