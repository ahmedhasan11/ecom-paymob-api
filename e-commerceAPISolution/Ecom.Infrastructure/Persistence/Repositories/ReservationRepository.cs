using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using Ecom.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Persistence.Repositories
{
	public class ReservationRepository : IReservationRepository
	{
		private readonly AppDbContext _db;
		public ReservationRepository(AppDbContext db)
		{
			_db= db;
		}
		public async Task CreateReservationAsync(InventoryReservation reservation, CancellationToken cancellationToken)
		{
			 await _db.InventoryReservations.AddAsync(reservation,cancellationToken);
		}

		public async Task<Dictionary<Guid, int>> GetActiveReservedQuantityBulkAsync(List<Guid> productIds, CancellationToken cancellationToken)
		{
			return await _db.InventoryReservations.Where(r => productIds.Contains(r.ProductId) && r.Status == ReservationStatusEnum.Active)
				.GroupBy(r => r.ProductId)
				.Select(g => new { ProductId = g.Key, ReservedQuantity = g.Sum(x => x.Quantity) }).ToDictionaryAsync(x=>x.ProductId, x=>x.ReservedQuantity, cancellationToken);
		}

		public async Task<int> GetActiveReservedQuantityByProductId(Guid productId,CancellationToken cancellationToken)
		{
			return await _db.InventoryReservations.Where(i=>i.ProductId==productId && i.Status==ReservationStatusEnum.Active).SumAsync(i=>i.Quantity, cancellationToken);
		}

		public IQueryable<InventoryReservation> GetReservationsByOrderId(Guid orderId)
		{
			return _db.InventoryReservations.Where(i=>i.OrderId==orderId).AsQueryable();
		}

		public async Task<bool> HasActiveReservationsAsync(Guid orderId , CancellationToken cancellationToken)
		{
			return await _db.InventoryReservations.AnyAsync(i => i.OrderId == orderId && i.Status == ReservationStatusEnum.Active&& i.ExpiresAt > DateTime.UtcNow);
		}
	}
}
