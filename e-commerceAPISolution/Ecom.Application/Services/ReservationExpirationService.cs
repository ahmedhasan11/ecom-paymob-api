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
	public class ReservationExpirationService : IReservationExpirationService
	{
		private readonly ILogger<ReservationExpirationService> _logger;
		private readonly IReservationRepository _reservationRepository;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IUnitOfWork _unitOfWork;
		public ReservationExpirationService( IReservationRepository reservationRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, ILogger<ReservationExpirationService> logger)
		{
			_unitOfWork = unitOfWork;
			_reservationRepository = reservationRepository;
			_paymentRepository = paymentRepository;
			_logger = logger;
		}

		public async Task ExpireReservationsAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting reservation expiration job");
			var reservations = await _reservationRepository.GetExpiredActiveReservationsForBackgroundJob(cancellationToken);
			if (!reservations.Any())
			{
				return;
			}
			var orderIds = reservations.Select(r => r.OrderId).Distinct().ToList();
			var succeededPayments = await _paymentRepository.GetSucceededPaymentsByOrderIdsInBulk(orderIds, cancellationToken);
			var succeededPaymentsDict = succeededPayments.ToLookup(p => p.OrderId);// Use ToLookup to handle multiple payment attempts (though unlikely)
			foreach (var reservation in reservations)
			{
				_logger.LogInformation("Processing reservation {ReservationId}", reservation.Id);
				if (reservation.Status!=ReservationStatusEnum.Active)
				{
					_logger.LogWarning("Skipping reservation {ReservationId} because it's not active", reservation.Id);
					continue;
				}
				//check payment status because of webhook race condition (payment can be completed while the job is running)	
				// Check if ANY payment for this order has succeeded
				if (succeededPaymentsDict.Contains(reservation.OrderId))
				{
					_logger.LogInformation("Skipping reservation {ReservationId} because payment is already succeeded", reservation.Id);
					continue;
				}
				reservation.Expire();
				_logger.LogInformation("Expired reservation {ReservationId}", reservation.Id);

				var order = reservation.Order;
				if (order != null && order.Status == OrderStatusEnum.Pending)
				{
					order.Cancel();
					_logger.LogInformation("Cancelled order {OrderId} due to reservation expiration", reservation.OrderId);
				}
			}
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			_logger.LogInformation("Finished reservation expiration job");
		}
	}
}
