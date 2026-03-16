using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
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
	}
}
