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
		Task AddProductAsync(Product product, CancellationToken cancellationToken); //we didnt return the product because this is add thing in repo ,return in Service

		//Task<IReadOnlyList<Product>> GetProductsAsync(ProductQueryOptions productQueryOptions, CancellationToken cancellationToken);

		IQueryable<Product> GetProductsSummaryAsync(ProductQueryOptions productQueryOptions);

		Task<int> GetTotalProductsCountAsync(ProductQueryOptions productQueryOptions, CancellationToken cancellationToken);

		Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);//we returned because its a read things so its normal to return
		Task<Product?> GetProductByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken); //to make the soft delete thing 
	}
}
