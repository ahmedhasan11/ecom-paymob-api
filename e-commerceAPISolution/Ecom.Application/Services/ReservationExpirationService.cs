using Ecom.Application.Interfaces;
using Ecom.Domain.Interfaces;
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
			//call repo method to get the reservations which is active and expired

			//check if the repo method result is empty

			//check that each reservation you got reservation.status==active

			//getpendingpayment by reservation.OrderId

			//chekc if payment is null

			//expire reservation

			//save changes
		}
	}
}
