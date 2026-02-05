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

		Task<IReadOnlyList<Product>> GetProductsAsync(string? search, decimal? minPrice, decimal? maxPrice,
			int pageNumber,	int pageSize); 

		Task<int> GetTotalProductsCountAsync(string? search, decimal? minPrice, decimal? maxPrice);

		Task<Product?> GetProductByIdAsync(Guid id);//we returned because its a read things so its normal to return

		Task UpdateProductAsync(Product product);

		Task DeleteProductAsync(Product product);



	}
}
