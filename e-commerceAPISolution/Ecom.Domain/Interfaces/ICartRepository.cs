using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface ICartRepository
	{
		IQueryable<Cart> GetCartQuery(Guid userId);
		Task<Cart?> GetMyCartAsync(Guid userId);
		Task AddCartAsync(Cart cart);
	}
}
