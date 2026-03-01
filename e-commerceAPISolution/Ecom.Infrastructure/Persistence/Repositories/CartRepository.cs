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
		public IQueryable<Cart> GetMyCartAsyc(Guid userId )
		{
			return _db.Carts.Where(x => x.UserId == userId); //bnrg3 el query nfson , el query hna lsa mt3mlo4 execute
		}
	}
}
