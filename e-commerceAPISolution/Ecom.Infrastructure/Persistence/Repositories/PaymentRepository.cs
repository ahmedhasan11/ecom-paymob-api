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
	public class PaymentRepository : IPaymentRepository
	{
		private readonly AppDbContext _db;
		public PaymentRepository(AppDbContext db)
		{
			_db = db;
		}
		public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
		{
			await _db.Payments.AddAsync(payment, cancellationToken);
		}
		public async Task<Payment?> GetPaymentByPaymentId(Guid paymentId, CancellationToken cancellationToken)
		{
			return await _db.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
		}
		public async Task<Payment?> GetPendingPaymentByOrderId(Guid orderId, CancellationToken cancellationToken)
		{
			return await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatusEnum.Pending,cancellationToken);
		}
		public async Task<Payment?> GetPaymentByPaymobOrderIdAsync(long paymobOrderId, CancellationToken cancellationToken)
		{
			return await _db.Payments.FirstOrDefaultAsync(p => p.PaymobOrderId == paymobOrderId, cancellationToken);
		}

		public async Task<List<Payment>> GetPendingPaymentsByOrderIdsInBulk(List<Guid> orderIds, CancellationToken cancellationToken)
		{
			return await _db.Payments.Where(p => orderIds.Contains(p.OrderId) && p.Status == PaymentStatusEnum.Pending).ToListAsync(cancellationToken);
		}
		public async Task<List<Payment>> GetExpiredPendingPaymentsAsync(DateTime now , DateTime threshold,  CancellationToken cancellationToken)
		{
			return await _db.Payments
				.Where(p => p.Status == PaymentStatusEnum.Pending &&
				(
					(p.ExpiresAt.HasValue && p.ExpiresAt <= now) ||
					(!p.ExpiresAt.HasValue && p.CreatedAt <= threshold)
				))
				.OrderBy(p=>p.CreatedAt)
				.Take(100)
				.ToListAsync(cancellationToken);
		}

		public async Task<List<Payment>> GetSucceededPaymentsByOrderIdsInBulk(List<Guid> orderIds, CancellationToken cancellationToken)
		{
			return await _db.Payments.Where(p => orderIds.Contains(p.OrderId) && p.Status == PaymentStatusEnum.Succeeded).ToListAsync(cancellationToken);
		}
	}
}
