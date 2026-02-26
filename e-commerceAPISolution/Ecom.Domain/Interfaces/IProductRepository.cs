using Ecom.Domain.Common.Queries;
using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IProductRepository
	{
		Task AddProductAsync(Product product); //we didnt return the product because this is add thing in repo ,return in Service

		Task<IReadOnlyList<Product>> GetProductsAsync(ProductQueryOptions productQueryOptions); 

		Task<int> GetTotalProductsCountAsync(ProductQueryOptions productQueryOptions);

		Task<Product?> GetProductByIdAsync(Guid id);//we returned because its a read things so its normal to return
	}
}
