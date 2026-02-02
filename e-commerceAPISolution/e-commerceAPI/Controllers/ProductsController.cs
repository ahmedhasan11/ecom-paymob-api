using Ecom.Application.DTOs.Products;
using Ecom.Application.Interfaces;
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
			if (id==Guid.Empty)
			{
				return Problem(title: "Invalid Request", detail: "Id cannot be empty", statusCode: StatusCodes.Status400BadRequest);
			}
			ProductDto? productDto = await _productservice.GetProductByIdAsync(id);

			if (productDto==null)
			{
				return Problem(title:"Product Not Found", detail: $"Product with id '{id}' was not found.", 
					statusCode:StatusCodes.Status404NotFound);
			}
			return Ok(productDto);
		}

		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAllProducts()
		{
			IReadOnlyList<ProductDto> productDtos = await _productservice.GetAllProductsAsync();
			return Ok(productDtos);
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
			if (id==Guid.Empty)
			{
				return Problem(title: "Invalid Request", detail: "Id cannot be empty", statusCode: StatusCodes.Status400BadRequest);
			}

			ProductDto? productDto = await _productservice.UpdateProductAsync(id, requestUpdateProductDto);
			if (productDto==null)
			{
				return Problem(title: "Product Not Found", detail: $"Product with id '{id}' was not found.",
					statusCode: StatusCodes.Status404NotFound);
			}
			return Ok(productDto);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(Guid id )
		{
			if (id==Guid.Empty)
			{
				return Problem(title: "Invalid Request", detail: "Id cannot be empty", statusCode: StatusCodes.Status400BadRequest);
			}

			bool is_deleted = await _productservice.DeleteProductAsync(id);

			if (is_deleted==false)
			{
				return Problem(title: "Product Not Found", detail: $"Product with id '{id}' was not found.",
					statusCode: StatusCodes.Status404NotFound);
			}

			return NoContent();
		}


	}
}
