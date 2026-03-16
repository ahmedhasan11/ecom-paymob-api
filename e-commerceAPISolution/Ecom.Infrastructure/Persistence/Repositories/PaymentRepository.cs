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
			return await _db.Payments.Where(p=>p.Id==paymentId).FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<Payment?> GetPendingPaymentByOrderId(Guid orderId, CancellationToken cancellationToken)
		{
			return await _db.Payments.AsNoTracking().Where(p => p.OrderId == orderId && p.Status == PaymentStatusEnum.Pending).FirstOrDefaultAsync(cancellationToken);
		}
	}
}
