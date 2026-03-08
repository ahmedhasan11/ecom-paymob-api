using Ecom.Application.Common.Pagination;
using Ecom.Domain.Common.Queries;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly AppDbContext _db;
		public OrderRepository(AppDbContext db)
		{
			_db= db;
		}
		public async Task AddOrderAsync(Order order, CancellationToken cancellationToken)
		{
			await _db.Orders.AddAsync(order, cancellationToken);
		}
		public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
		{
			return await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
		}
		public IQueryable<Order> GetUserOrdersQuery(Guid userId)
		{
			return _db.Orders.Where(o=>o.UserId==userId);
		}
		public IQueryable<Order> GetOrderDetailsQuery(Guid userId, Guid orderId)
		{
			return _db.Orders.Where(o => o.UserId == userId && o.Id == orderId);
		}
	}
}
