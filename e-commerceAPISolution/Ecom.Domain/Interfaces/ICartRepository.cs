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
		IQueryable<Cart> GetMyCartAsyc(Guid userId);
	}
}
