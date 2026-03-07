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
		Task<CartResultDto> GetMyCartAsync(Guid userId, CancellationToken cancellationToken);

		Task<CartResultDto> AddItemToCartAsync(Guid userId ,RequestAddToCartDto dto, CancellationToken cancellationToken);

		Task<CartResultDto> RemoveItemFromCartAsync(Guid userId ,Guid productId, CancellationToken cancellationToken);

		Task<CartResultDto> UpdateCartItemQuantityAsync(Guid userId ,Guid productId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken);

		Task ClearCartAsync(Guid userId, CancellationToken cancellationToken);
	}
}
