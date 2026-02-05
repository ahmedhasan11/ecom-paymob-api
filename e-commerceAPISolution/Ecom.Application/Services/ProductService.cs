using Ecom.Application.Common.Filtering;
using Ecom.Application.Common.Pagination;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Ecom.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
		{
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
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
			int maxAllowed = 50;
			int defaultpageSize = 10;
			int pageNumber = productQueryParams.pageNumber;
			int pageSize = productQueryParams.pageSize;
			string? search = productQueryParams.search;
			decimal? minPrice = productQueryParams.MinPrice;
			decimal? maxPrice = productQueryParams.MaxPrice;

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

			int TotalProductsCount = await _productRepository.GetTotalProductsCountAsync(search , minPrice , maxPrice);

			IReadOnlyList<Product> products= await _productRepository.GetProductsAsync(search, minPrice,
				maxPrice , pageNumber , pageSize);

			IReadOnlyList<ProductDto> productDtos= products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price.Amount, CreatedAt = p.CreatedAt, ImageUrl= p.ImageUrl, Description=p.Description }).ToList();

			PagedResult<ProductDto> pagedResult = new PagedResult<ProductDto>() 
			{
				Items = productDtos,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = TotalProductsCount,
				TotalPages = (int)Math.Ceiling((double)TotalProductsCount / pageSize) 
			};
			return pagedResult;
		}	 
		public async Task<ProductDto?> GetProductByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("id cannot be empty", nameof(id));
			}

			Product? product = await _productRepository.GetProductByIdAsync(id);
			if (product == null)
			{
				//throw new ArgumentException("there is no product with this id"); 
				//notice: this is not an exception , exception is for programmateic error
				//but the status we are in is just a not found
				//return null;
				throw new NotFoundException($"Product with {id} was not found ");
			}

			return new ProductDto() { Id = product.Id, Name = product.Name, Price = product.Price.Amount, CreatedAt = product.CreatedAt, Description= product.Description, ImageUrl= product.ImageUrl };
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
