using Ecom.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface ICheckoutService
	{
		Task<Guid> CheckoutAsync(Guid userId, CancellationToken cancellationToken, ShippingAddressDto address);
	}
}
