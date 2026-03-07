using Ecom.Application.Common.Filtering;
using Ecom.Application.Common.Pagination;
using Ecom.Application.DTOs.Products;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productservice;
		public ProductsController(IProductService productService) { _productservice = productService; }

		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductDto>> GetProductById(Guid id, CancellationToken cancellationToken)
		{
			ProductDto? productDto = await _productservice.GetProductByIdAsync(id, cancellationToken);
			return Ok(productDto);
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery]ProductQueryParams productQueryParams, CancellationToken cancellationToken)
		{
			PagedResult<ProductDto> pagedResult = await _productservice.GetProductsAsync(productQueryParams, cancellationToken);
			return Ok(pagedResult);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost]
		public async Task<ActionResult<ProductDto>> AddProduct(RequestAddProductDto requestAddProductDto, CancellationToken cancellationToken)
		{
			ProductDto productDto = await _productservice.AddProductAsync(requestAddProductDto, cancellationToken);
			return CreatedAtAction(nameof(GetProductById), new { id= productDto.Id}, productDto);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{id}")]
		public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id , RequestUpdateProductDto requestUpdateProductDto, CancellationToken cancellationToken)
		{
			ProductDto? productDto = await _productservice.UpdateProductAsync(id, requestUpdateProductDto, cancellationToken);
			return Ok(productDto);
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
		{
			await _productservice.DeleteProductAsync(id, cancellationToken);
			return NoContent();
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{id}/stock/increase")]
		public async Task<IActionResult> IncreaseStock(Guid id,UpdateStockDto dto, CancellationToken cancellationToken)
		{
			await _productservice.IncreaseStockAsync(id,dto, cancellationToken);
			return NoContent();
		}
		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{id}/stock/decrease")]
		public async Task<IActionResult> DecreaseStock(Guid id, UpdateStockDto dto, CancellationToken cancellationToken)
		{
			await _productservice.DecreaseStockAsync(id,dto, cancellationToken);
			return NoContent();
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{id}/availability")]
		public async Task<IActionResult> ToggleAvailability(Guid id, ToggleAvailabilityDto dto, CancellationToken cancellationToken)
		{

			await _productservice.ToggleAvailabilityAsync(id,dto, cancellationToken);
			return NoContent();
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{id}/restore")]
		public async Task<IActionResult> RestoreProduct(Guid id, CancellationToken cancellationToken)
		{
			await _productservice.RestoreProductAsync(id, cancellationToken);
			return NoContent();
		}
	}
}
