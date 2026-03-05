using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
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
		public async Task AddOrderAsync(Order order)
		{
			await _db.Orders.AddAsync(order);
		}
	}
}
