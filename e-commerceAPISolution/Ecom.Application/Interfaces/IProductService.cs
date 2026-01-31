using Ecom.Application.DTOs.Products;
using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IProductService
	{
		Task<ProductDto> AddProductAsync(RequestAddProductDto requestAddProductDto);

		Task<IReadOnlyList<ProductDto>> GetAllProductsAsync();

		Task<ProductDto?> GetProductByIdAsync(Guid id);

		Task<ProductDto?> UpdateProductAsync(Guid id ,RequestUpdateProductDto requestupdateProductDto );

		Task<bool> DeleteProductAsync(Guid id);
	}
}
