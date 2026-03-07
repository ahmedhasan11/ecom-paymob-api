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
		Task<ProductDto> AddProductAsync(RequestAddProductDto requestAddProductDto, CancellationToken cancellationToken);

		Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams productQueryParams, CancellationToken cancellationToken);

		Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<ProductDto?> UpdateProductAsync(Guid id ,RequestUpdateProductDto requestupdateProductDto, CancellationToken cancellationToken);

		Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);

		Task IncreaseStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken);
		Task DecreaseStockAsync(Guid id, UpdateStockDto dto, CancellationToken cancellationToken);

		Task ToggleAvailabilityAsync(Guid id, ToggleAvailabilityDto dto, CancellationToken cancellationToken);

		Task RestoreProductAsync(Guid id, CancellationToken cancellationToken);
	}
}
