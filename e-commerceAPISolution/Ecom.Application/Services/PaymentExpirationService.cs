using Ecom.Application.Interfaces;
using Ecom.Domain.Enums;
using Ecom.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class PaymentExpirationService:IPaymentExpirationService
	{
		private readonly IPaymentRepository _paymentRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<PaymentExpirationService> _logger;
		public PaymentExpirationService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, ILogger<PaymentExpirationService> logger)
		{
			_paymentRepository = paymentRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}
		public async Task ExpirePaymentsAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting payment expiration job");
			var now = DateTime.UtcNow;
			var threshold = now.AddMinutes(-10); // configurable


			var payments = await _paymentRepository
				.GetExpiredPendingPaymentsAsync( now, threshold, cancellationToken);

			if (!payments.Any())
				return;

			foreach (var payment in payments)
			{
				_logger.LogInformation("Processing payment {PaymentId}", payment.Id);

				if (payment.Status != PaymentStatusEnum.Pending)
					continue;

				payment.MarkAsFailed(null);
				_logger.LogInformation("Marked payment {PaymentId} as failed", payment.Id);
			}

			// 5) save changes
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
