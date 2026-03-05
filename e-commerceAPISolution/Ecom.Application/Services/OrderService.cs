using Ecom.Application.DTOs.Order;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Ecom.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;
		public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
		{ 
			_orderRepository = orderRepository;
			_unitOfWork = unitOfWork;
		}
		public async Task<OrderResult> UpdateOrderStatusAsync(Guid orderId, OrderStatusRequestDto dto)
		{
			if (orderId==Guid.Empty)
			{
				throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
			}
			Order? order = await _orderRepository.GetOrderByIdAsync(orderId);
			if (order==null)
			{
				throw new NotFoundException("order with this id not found");
			}
			if ( dto.Status==OrderStatusEnum.PaymentFailed)
			{
				order.MarkAsPaymentFailed();
			}
			else if (dto.Status == OrderStatusEnum.Paid)
			{
				order.MarkAsPaid();
			}
			else if (dto.Status==OrderStatusEnum.Cancelled)
			{
				order.Cancel();
			}
			else
			{
				throw new ArgumentException("Invalid order status");
			}
			await _unitOfWork.SaveChangesAsync();
			return new OrderResult {OrderId=orderId, Status=order.Status , Total= order.TotalAmount};
		}
	}
}
