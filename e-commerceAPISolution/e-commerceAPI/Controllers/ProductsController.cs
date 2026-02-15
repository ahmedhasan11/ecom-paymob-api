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

		#region Preparing 
		//Add Product ---> ActionResult --> returning product obj               [POST]              //DONE
		//Update Product ---> ActionResult --> returning product obj            [Patch]
		//Delete Product ---> IActionResult --> returning NoCOntent             [Delete]
		//GetProductById ---> ActionResult --> returning product obj            [GET]               //DONE
		//GetAllProducts ---> ActionResult --> returning objects				[GET]              //DONE 
		#endregion

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
		{
			ProductDto? productDto = await _productservice.GetProductByIdAsync(id);
			return Ok(productDto);
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery]ProductQueryParams productQueryParams)
		{
			PagedResult<ProductDto> pagedResult = await _productservice.GetProductsAsync(productQueryParams);
			return Ok(pagedResult);
		}

		[HttpPost]
		public async Task<ActionResult<ProductDto>> AddProduct(RequestAddProductDto requestAddProductDto)
		{
			ProductDto productDto = await _productservice.AddProductAsync(requestAddProductDto);
			return CreatedAtAction(nameof(GetProductById), new { id= productDto.Id}, productDto);
		}

		[HttpPatch("{id}")]
		public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id , RequestUpdateProductDto requestUpdateProductDto)
		{
			ProductDto? productDto = await _productservice.UpdateProductAsync(id, requestUpdateProductDto);
			return Ok(productDto);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id )
		{
			await _productservice.DeleteProductAsync(id);
			return NoContent();
		}


	}
}
