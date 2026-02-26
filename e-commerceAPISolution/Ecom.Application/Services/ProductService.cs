using Ecom.Application.Common.Filtering;
using Ecom.Application.Common.Pagination;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Common.Queries;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace Ecom.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICacheService _cacheService;
		private readonly ILogger<ProductService> _logger;
		public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<ProductService> logger)
		{
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
			_cacheService = cacheService;
			_logger = logger;
		}

		private async Task InvalidateProductsCacheAsync(string reason)
		{
			await _cacheService.RemoveByPrefixAsync("products:");
			_logger.LogInformation("Products cache invalidated. Reason: {Reason}", reason);
		}
		public async Task<ProductDto> AddProductAsync(RequestAddProductDto requestAddProductDto)
		{
			if (requestAddProductDto == null)
			{
				_logger.LogWarning("AddProduct operation received a null request payload");
				throw new ArgumentNullException(nameof(requestAddProductDto));
			}

			_logger.LogInformation("Starting product creation with Name={Name} and Price={Price}",
			requestAddProductDto.Name, requestAddProductDto.Price);			
			Product product= new Product(requestAddProductDto.Price!.Value, requestAddProductDto.Name!, requestAddProductDto.Stock);
			product.ImageUrl = requestAddProductDto.ImageUrl;
			product.Description = requestAddProductDto.Description;

			#region notes
			//product.Id= Guid.NewGuid();
			//product.Name= requestAddProductDto.Name;
			//product.Price= Money.From( requestAddProductDto.Price);
			//أي فلوس تدخل الدومين لازم تعدّي على Money.From عشان أتأكد إنها فلوس صح، مش رقم وخلاص
			//_logger.LogInformation("going inside Repo"); 
			#endregion
			await _productRepository.AddProductAsync(product);
			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Product Added");

			_logger.LogInformation("Product created successfully with Id={ProductId}", product.Id);

			return new ProductDto() { Id = product.Id, Name = product.Name, Price = product.Price.Amount, CreatedAt = product.CreatedAt
				, Description= product.Description, ImageUrl= product.ImageUrl, IsAvailable= product.IsAvailable, IsInStock= product.IsInStock };
		}
		public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParams productQueryParams)
		{
			#region Paging vars
			int maxAllowed = 50;
			int defaultpageSize = 10;
			int pageNumber = productQueryParams.pageNumber;
			int pageSize = productQueryParams.pageSize;
			#endregion
			#region Filtering vars
			string? search = productQueryParams.search;
			decimal? minPrice = productQueryParams.MinPrice;
			decimal? maxPrice = productQueryParams.MaxPrice;
			#endregion
			#region Sorting vars
			string? sortBy = productQueryParams.sortBy;
			string? sortOrder = productQueryParams.sortOrder;
			#endregion

			#region Filtering Conditions
			if (minPrice.HasValue && minPrice.Value < 0)
			{
				minPrice = null;
			}
			if (maxPrice.HasValue && maxPrice.Value < 0)
			{
				maxPrice = null;
			}
			if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
			{
				var temp = minPrice;
				minPrice = maxPrice;
				maxPrice = temp;
			}
			#endregion
			#region Paging Conditions
			if (pageNumber < 1)
			{
				//throw new ArgumentException("pageNumber or pageSize values are Invalid");
				//pagination doesnt throw an exception its a show logic
				pageNumber = 1;
			}
			if (pageSize > maxAllowed)
			{
				pageSize = maxAllowed;
			}
			if (pageSize < 1)
			{
				pageSize = defaultpageSize;
			}
			#endregion
			#region Sorting Conditions
			//SortBy
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				sortBy = sortBy.ToLower();
			}
			switch (sortBy)
			{
				case "price":
					break;
				case "name":
					break;
				case "createdat":
					break;
				default:
					sortBy = "createdat";
					break;
			}
			//SortOrder
			if (!string.IsNullOrWhiteSpace(sortOrder))
			{
				sortOrder = sortOrder.ToLower();
			}
			if (string.IsNullOrWhiteSpace(sortOrder))
			{
				if (sortBy == "createdat")
				{
					sortOrder = "desc";
				}
				else
				{
					sortOrder = "asc";
				}

			}
			else if (sortOrder.Equals("desc"))
			{
				sortOrder = "desc";
			}
			else
			{
				sortOrder = "asc";
			}
			#endregion

			_logger.LogInformation("Fetching products with filters: Search={Search}, MinPrice={Min}, MaxPrice={Max}, Page={Page}, Size={Size}, SortBy={SortBy}, SortOrder={SortOrder}",
			search, minPrice, maxPrice, pageNumber, pageSize,sortBy,sortOrder);

			#region Redis Caching
			var key = $"products:search={search}" +
		$":min={minPrice}:max={maxPrice}:sort={sortBy}:order={sortOrder}:page={pageNumber}:size={pageSize}";

			var cachedJson = await _cacheService.GetAsync<PagedResult<ProductDto>>(key);
			if (cachedJson != null)
			{
					_logger.LogInformation("Cache HIT for products list. Key={CacheKey}", key);
					return cachedJson;
				
			}
			#endregion
			_logger.LogInformation("Cache MISS for products list. Key={CacheKey}. Fetching from database...", key);
			ProductQueryOptions productQueryOptions = new ProductQueryOptions()
			{
			 search= search,
			 minPrice= minPrice,
			 maxPrice= maxPrice,
			 pageNumber= pageNumber,
			 pageSize= pageSize,
			 sortBy=sortBy,
			 sortOrder=sortOrder,
			};

			int TotalProductsCount = await _productRepository.GetTotalProductsCountAsync(productQueryOptions);

			IReadOnlyList<Product> products= await _productRepository.GetProductsAsync(productQueryOptions);

			_logger.LogInformation("Fetched {Count} products from database", products.Count);

			IReadOnlyList<ProductDto> productDtos= products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price.Amount, CreatedAt = p.CreatedAt, ImageUrl= p.ImageUrl, Description=p.Description, IsAvailable = p.IsAvailable, IsInStock = p.IsInStock }).ToList();

			PagedResult<ProductDto> pagedResult = new PagedResult<ProductDto>() 
			{
				Items = productDtos,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = TotalProductsCount,
				TotalPages = (int)Math.Ceiling((double)TotalProductsCount / pageSize) 
			};


			await _cacheService.SetAsync(key, pagedResult, TimeSpan.FromMinutes(1));
			_logger.LogInformation("Products list cached successfully. Key={CacheKey}", key);

			return pagedResult;
		}	 
		public async Task<ProductDto?> GetProductByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				_logger.LogWarning("GetProductById received an empty ProductId");
				throw new ArgumentException("id cannot be empty", nameof(id));
			}

			_logger.LogInformation(
				"Starting GetProductById operation. ProductId={ProductId}", id);

			#region RedisCaching
			var key = $"products:{id}";
			var cachedJson = await _cacheService.GetAsync<ProductDto>(key);
			if (cachedJson != null)
			{
					_logger.LogInformation(
						"Cache HIT for product. ProductId={ProductId}, CacheKey={CacheKey}",
						id, key);

					return cachedJson;
				
			}
			#endregion
			_logger.LogInformation(
				"Cache MISS for product. ProductId={ProductId}. Fetching from database",
				id);

			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				//throw new ArgumentException("there is no product with this id"); 
				//notice: this is not an exception , exception is for programmateic error
				//but the status we are in is just a not found
				//return null;

				_logger.LogWarning("Product not found. ProductId={ProductId}", id);

				throw new NotFoundException($"Product with {id} was not found ");
			}
			
			var productdto = new ProductDto()
			{
				Id = product.Id,
				Name = product.Name,
				Price = product.Price.Amount,
				CreatedAt = product.CreatedAt,
				Description = product.Description,
				ImageUrl = product.ImageUrl,
				IsAvailable = product.IsAvailable,
				IsInStock = product.IsInStock
			};

			await _cacheService.SetAsync(key,productdto,TimeSpan.FromMinutes(2));
			_logger.LogInformation(
				"Product cached successfully. ProductId={ProductId}, CacheKey={CacheKey}",
				id, key);

			return productdto;

		}
		public async Task<ProductDto?> UpdateProductAsync(Guid id, RequestUpdateProductDto requestupdateProductDto)
		{

			if (id==Guid.Empty)
			{
				_logger.LogWarning("UpdateProduct received an empty ProductId");

				throw new ArgumentException("ID cannot be empty", nameof(id));
			}

			if (requestupdateProductDto==null)
			{
				_logger.LogWarning("UpdateProduct request payload was null. ProductId={ProductId}", id);

				throw new ArgumentNullException("dto is null", nameof(requestupdateProductDto));
			}
			_logger.LogInformation(
			"Starting UpdateProduct operation. ProductId={ProductId}", id);

			Product? product = await _productRepository.GetProductByIdAsync(id);

			#region Values Checks
			if (product == null)
			{
				_logger.LogWarning("Product not found for update. ProductId={ProductId}", id);
				throw new NotFoundException($"Product with {id} was not found ");
			}
			if (requestupdateProductDto.Name is not null)
			{
				product.Name = requestupdateProductDto.Name;
			}
			if (requestupdateProductDto.Price.HasValue)
			{
				product.Price = Money.From(requestupdateProductDto.Price.Value);
			}
			if (requestupdateProductDto.ImageUrl is not null)
			{
				product.ImageUrl = requestupdateProductDto.ImageUrl;
			}
			if (requestupdateProductDto.Description is not null)
			{
				product.Description = requestupdateProductDto.Description;
			} 
			#endregion

			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Product Updated");

			_logger.LogInformation(
			"Product updated successfully. ProductId={ProductId}", product.Id);

			return new ProductDto() { Id= product.Id, Name= product.Name ,Price= product.Price.Amount, CreatedAt = product.CreatedAt , ImageUrl= product.ImageUrl , Description= product.Description, IsAvailable = product.IsAvailable, IsInStock = product.IsInStock };

		}
		public async Task<bool> DeleteProductAsync(Guid id)
		{
			if (id==Guid.Empty)
			{
				_logger.LogWarning("DeleteProduct received an empty ProductId");
				throw new ArgumentException("ID cannot be empty", nameof(id));
			}

			_logger.LogInformation(
			"Starting DeleteProduct operation. ProductId={ProductId}", id);

			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product==null)
			{
				_logger.LogWarning("Product not found for deletion. ProductId={ProductId}", id);

				throw new NotFoundException($"Product with {id} was not found ");
			}

			_logger.LogInformation(
				"Deleting product. ProductId={ProductId}, ProductName={ProductName}",
				product.Id, product.Name);

			product.SoftDelete();
			await _unitOfWork.SaveChangesAsync();

			await InvalidateProductsCacheAsync("Product Soft Deleted");

			_logger.LogInformation("Product deleted successfully. ProductId={ProductId}", id);

			return true;
		}
		public async Task IncreaseStockAsync(Guid id,UpdateStockDto dto)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty.");
			}
			_logger.LogInformation(	"Starting IncreaseStock operation. ProductId={ProductId}, Quantity={Quantity}",
			id, dto.Quantity);
			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product==null)
			{
				_logger.LogWarning("Product not found for Increase stock. ProductId={ProductId}", id);
				throw new NotFoundException($"Product with {id} was not found ");
			}
			product.IncreaseStock(dto.Quantity);
			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Product Stock Increase");
			_logger.LogInformation(	"Stock increased successfully. ProductId={ProductId}, NewStock={Stock}",
				product.Id, product.StockQuantity);
			return;
		}
		public async Task DecreaseStockAsync(Guid id, UpdateStockDto dto)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty.");
			}
			_logger.LogInformation("Starting DecreaseStock operation. ProductId={ProductId}, Quantity={Quantity}",
			id, dto.Quantity);
			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				_logger.LogWarning("Product not found for Decrease stock. ProductId={ProductId}", id);
				throw new NotFoundException($"Product with {id} was not found ");
			}
			product.DecreaseStock(dto.Quantity);
			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Product Stock Decrease");
			_logger.LogInformation("Stock decreased successfully. ProductId={ProductId}, NewStock={Stock}",
			product.Id, product.StockQuantity);
			return;
		}
		public async Task ToggleAvailabilityAsync(Guid id, ToggleAvailabilityDto dto)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty.");
			}
			_logger.LogInformation("Starting ToggleAvailability operation. ProductId={ProductId}, RequestedAvailability={Availability}",
			id, dto.Available);
			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				_logger.LogWarning("Product not found for Toggle Availability. ProductId={ProductId}", id);
				throw new NotFoundException($"Product with {id} was not found ");
			}

			if (product.IsAvailable==dto.Available)
			{
				_logger.LogInformation(	"ToggleAvailability skipped. Product already in requested state. ProductId={ProductId}, Availability={Availability}",
				product.Id, product.IsAvailable);
				return;
			}

			if (dto.Available==true)			
				product.MakeAvailable();
			else 
				product.MakeUnavailable();


			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Toggle Product Availability");
			_logger.LogInformation("Product availability updated successfully. ProductId={ProductId}, NewAvailability={Availability}",
			product.Id, product.IsAvailable);
			return;
		}
		public async Task RestoreProductAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty.");
			}
			_logger.LogInformation(	"Starting RestoreProduct operation. ProductId={ProductId}",	id);
			Product? product = await _productRepository.GetProductByIdIncludingDeletedAsync(id);
			if (product == null)
			{
				_logger.LogWarning("Product not found for Restore. ProductId={ProductId}", id);
				throw new NotFoundException($"Product with {id} was not found ");
			}
			if (product.IsDeleted==false)
			{
				_logger.LogInformation("Restore skipped. Product is not deleted. ProductId={ProductId}",product.Id);
				return;
			}
			product.Restore();
			await _unitOfWork.SaveChangesAsync();
			await InvalidateProductsCacheAsync("Restore Product");
			_logger.LogInformation(	"Product restored successfully. ProductId={ProductId}",	product.Id);
			return;
		}
	}
}
