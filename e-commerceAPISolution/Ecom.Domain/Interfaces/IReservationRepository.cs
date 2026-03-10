using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Interfaces
{
	public interface IReservationRepository
	{
		Task CreateReservationAsync(InventoryReservation reservation,CancellationToken cancellationToken);

		IQueryable<InventoryReservation> GetReservationsByOrderId(Guid orderId);

		Task<int> GetActiveReservedQuantityByProductId(Guid productId, CancellationToken cancellationToken);

		//Task GetExpiredReservationAsync();
	}
}
