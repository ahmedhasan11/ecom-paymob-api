using Ecom.Application.DTOs.Products;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;
		public ProductService(IProductRepository productRepository)
		{
		_productRepository = productRepository;
		}
		public async Task<ProductDto> AddProductAsync(RequestAddProductDto requestAddProductDto)
		{
			if (requestAddProductDto == null)
			{
				throw new ArgumentNullException(nameof(requestAddProductDto));
			}
			Product product= new Product(requestAddProductDto.Price, requestAddProductDto.Name);
			//product.Id= Guid.NewGuid();
			//product.Name= requestAddProductDto.Name;
			//product.Price= Money.From( requestAddProductDto.Price);
			//أي فلوس تدخل الدومين لازم تعدّي على Money.From عشان أتأكد إنها فلوس صح، مش رقم وخلاص

			await _productRepository.AddProductAsync(product);

			return new ProductDto() { Id = product.Id, Name = product.Name, Price = product.Price.Amount, CreatedAt = product.CreatedAt };
		}

		public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync()
		{
			IReadOnlyList<Product> products= await _productRepository.GetAllProductsAsync();
			IReadOnlyList<ProductDto> productDtos= products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price.Amount, CreatedAt = p.CreatedAt }).ToList();
			return productDtos;
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
				return null;
			}

			return new ProductDto() { Id = product.Id, Name = product.Name, Price = product.Price.Amount, CreatedAt = product.CreatedAt };
		}
	}
}
