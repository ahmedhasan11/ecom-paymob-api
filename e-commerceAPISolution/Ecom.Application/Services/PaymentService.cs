using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IPaymentRepository _paymentRepository;
		private readonly IPaymentGateway _paymentGateway;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrderRepository _orderRepository;
		public PaymentService(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway, IUnitOfWork unitOfWork, IOrderRepository orderRepository)
		{
			_paymentRepository = paymentRepository;
			_paymentGateway = paymentGateway;
			_unitOfWork = unitOfWork;
			_orderRepository = orderRepository;
		}
		public async Task<PaymentSessionResponse> CreatePaymentSessionAsync(Guid orderId, CancellationToken cancellationToken)
		{
			if (orderId == Guid.Empty)
			{
				throw new ArgumentException("orderId cannot be empty.",nameof(orderId));
			}
			//Load Order
			//order = GetById(orderId)


			//Validate Order
			/*if order == null → throw

				if order.Status == Paid → throw

				if order.Status == Cancelled → throw*/

			//check if reservations still active(for multiple payments use case)

			//check pending payment
			//if there is pending payment , check if its not expired , if not expired --> return checkoutUrl
			//if payment expired --> create new payment entity
			//if no pending payment --> create new payment entity
			//if there is already pending payment --> return the payment session url
			//add payment record to db 
			//save changes 3shan n2dr nb3t l paymob el merchant == payment.id
			//call the payment gateway 
			// check if payment gateway throws an error --> payment.MarkAsFailed() , dont make the reservation release, save changes , throw exception
			//get the paymob gateway response 
			// var expiresAt = datetime.utcnow.addseconds(_paymob.ExpirationSeconds)
			
			//payment.savepaymobdata(checkouturl, paymoborderid, expiresAt)
			//save changes
			//return checkout url from the payment gateway


		}
	}
}
