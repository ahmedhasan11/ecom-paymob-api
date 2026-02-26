using Ecom.Application.Common.Filtering;
using Ecom.Application.Common.Pagination;
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

		Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams productQueryParams);

		Task<ProductDto?> GetProductByIdAsync(Guid id);

		Task<ProductDto?> UpdateProductAsync(Guid id ,RequestUpdateProductDto requestupdateProductDto );

		Task<bool> DeleteProductAsync(Guid id);

		Task IncreaseStockAsync(UpdateStockDto dto);
		Task DecreaseStockAsync(UpdateStockDto dto);

		Task ToggleAvailabilityAsync(ToggleAvailabilityDto dto);

		Task RestoreProductAsync(Guid id);
	}
}
