using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class ReservationExpirationService : IReservationExpirationService
	{

		private readonly IReservationRepository _reservationRepository;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IUnitOfWork _unitOfWork;
		public ReservationExpirationService( IReservationRepository reservationRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_reservationRepository = reservationRepository;
			_paymentRepository = paymentRepository;
		}

		public async Task ExpireReservationsAsync(CancellationToken cancellationToken)
		{
			var reservations = await _reservationRepository.GetExpiredActiveReservationsForBackgroundJob(cancellationToken);
			if (!reservations.Any())
			{
				return;
			}
			foreach (var reservation in reservations)
			{
				if (reservation.Status!=ReservationStatusEnum.Active)
				{
					continue;
				}
				//check payment status because of webhook race codnition (payment can be completed while the job is running)	
				var payment = await _paymentRepository.GetPendingPaymentByOrderId(reservation.OrderId, cancellationToken);
				if (payment == null)
				{
					continue; //(payment already succeeded/failed)
				}
				reservation.Expire();
			}
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}
}
