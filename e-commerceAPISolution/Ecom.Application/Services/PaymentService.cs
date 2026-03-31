using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
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
			throw new NotImplementedException();
		}
	}
}
