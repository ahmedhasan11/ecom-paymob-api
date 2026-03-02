using Ecom.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface ICartService
	{
		Task<CartResultDto> GetMyCartAsync(Guid userId);

		Task<CartResultDto> AddItemToCartAsync(Guid userId ,RequestAddToCartDto dto);

		Task<CartResultDto> RemoveItemFromCartAsync(Guid userId ,Guid productId);
	}
}
