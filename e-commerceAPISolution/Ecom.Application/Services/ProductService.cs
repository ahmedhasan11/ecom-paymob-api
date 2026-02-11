using Ecom.Application.Common.Filtering;
using Ecom.Application.Common.Pagination;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Common.Queries;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
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
		public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
		{
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
			_cacheService = cacheService;
		}
		public async Task<ProductDto> AddProductAsync(RequestAddProductDto requestAddProductDto)
		{
			if (requestAddProductDto == null)
			{
				throw new ArgumentNullException(nameof(requestAddProductDto));
			}
			Product product= new Product(requestAddProductDto.Price, requestAddProductDto.Name);
			product.ImageUrl = requestAddProductDto.ImageUrl;
			product.Description = requestAddProductDto.Description;

			//product.Id= Guid.NewGuid();
			//product.Name= requestAddProductDto.Name;
			//product.Price= Money.From( requestAddProductDto.Price);
			//أي فلوس تدخل الدومين لازم تعدّي على Money.From عشان أتأكد إنها فلوس صح، مش رقم وخلاص

			await _productRepository.AddProductAsync(product);
			await _unitOfWork.SaveChangesAsync();

			return new ProductDto() { Id = product.Id, Name = product.Name, Price = product.Price.Amount, CreatedAt = product.CreatedAt , Description= product.Description, ImageUrl= product.ImageUrl };
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

			#region Redis Caching
			var key = $"products:search={search}" +
		$":min={minPrice}:max={maxPrice}:sort={sortBy}:order={sortOrder}:page={pageNumber}:size={pageSize}";

			var cachedJson = await _cacheService.GetAsync(key);
			if (cachedJson != null)
			{
				PagedResult<ProductDto>? result = JsonSerializer.Deserialize<PagedResult<ProductDto>>(cachedJson);
				if (result != null)
				{
					return result;
				}
			} 
			#endregion

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

			IReadOnlyList<ProductDto> productDtos= products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price.Amount, CreatedAt = p.CreatedAt, ImageUrl= p.ImageUrl, Description=p.Description }).ToList();

			PagedResult<ProductDto> pagedResult = new PagedResult<ProductDto>() 
			{
				Items = productDtos,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = TotalProductsCount,
				TotalPages = (int)Math.Ceiling((double)TotalProductsCount / pageSize) 
			};

			string JsonToCache = JsonSerializer.Serialize(pagedResult);
			await _cacheService.SetAsync(key, JsonToCache, TimeSpan.FromMinutes(1));
			return pagedResult;
		}	 
		public async Task<ProductDto?> GetProductByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("id cannot be empty", nameof(id));
			}

			#region RedisCaching
			var key = $"products:{id}";
			var cachedJson = await _cacheService.GetAsync(key);
			if (cachedJson != null)
			{
				var result = JsonSerializer.Deserialize<ProductDto>(cachedJson);
				if (result != null)
				{
					return result;
				}
			} 
			#endregion

			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				//throw new ArgumentException("there is no product with this id"); 
				//notice: this is not an exception , exception is for programmateic error
				//but the status we are in is just a not found
				//return null;
				throw new NotFoundException($"Product with {id} was not found ");
			}

			var productdto = new ProductDto()
			{
				Id = product.Id,
				Name = product.Name,
				Price = product.Price.Amount,
				CreatedAt = product.CreatedAt,
				Description = product.Description,
				ImageUrl = product.ImageUrl
			};
			var jsontocache = JsonSerializer.Serialize(productdto);
			await _cacheService.SetAsync(key,jsontocache,TimeSpan.FromMinutes(2));

			return productdto;

		}
		public async Task<ProductDto?> UpdateProductAsync(Guid id, RequestUpdateProductDto requestupdateProductDto)
		{
			if (id==Guid.Empty)
			{
				throw new ArgumentException("ID cannot be empty", nameof(id));
			}

			if (requestupdateProductDto==null)
			{
				throw new ArgumentNullException("dto is null", nameof(requestupdateProductDto));
			}

			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product==null)
			{
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
			await _unitOfWork.SaveChangesAsync();

			return new ProductDto() { Id= product.Id, Name= product.Name ,Price= product.Price.Amount, CreatedAt = product.CreatedAt , ImageUrl= product.ImageUrl , Description= product.Description};

		}
		public async Task<bool> DeleteProductAsync(Guid id)
		{
			if (id==Guid.Empty)
			{
				throw new ArgumentException("ID cannot be empty", nameof(id));
			}
			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product==null)
			{
				throw new NotFoundException($"Product with {id} was not found ");
			}
			await _productRepository.DeleteProductAsync(product);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}
	}
}
