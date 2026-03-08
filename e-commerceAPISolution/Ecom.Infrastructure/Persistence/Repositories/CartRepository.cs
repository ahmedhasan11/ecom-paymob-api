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
	public class CartRepository : ICartRepository
	{
		private readonly AppDbContext _db;
		public CartRepository(AppDbContext db)
		{
			_db = db;
		}

		public async Task AddCartAsync(Cart cart, CancellationToken cancellationToken)
		{
			await _db.Carts.AddAsync(cart,cancellationToken);
		}

		public IQueryable<Cart> GetCartQuery(Guid userId)
		{
			return _db.Carts.Where(x => x.UserId == userId); //bnrg3 el query nfson , el query hna lsa mt3mlo4 execute
		}

		public async Task<Cart?> GetMyCartAsync(Guid userId, CancellationToken cancellationToken)
		{
			return await _db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c=>c.UserId==userId,cancellationToken);
		}
	}
}
