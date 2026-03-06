using Ecom.Application.DTOs.Order;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Ecom.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
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
			return new OrderResult {OrderId=orderId, Status=order.Status , Total= order.TotalAmount, CreatedAt= order.CreatedAt};
		}
		public async Task<List<OrderResult>> GetUserOrdersSummaryAsync(Guid userId)
		{
			if (userId==Guid.Empty) 
			{
				throw new ArgumentException("user id cannot be empty.", nameof(userId));
			}
			var query = _orderRepository.GetUserOrdersQuery(userId);

			List<OrderResult> orders = await query.OrderByDescending(o=>o.CreatedAt).Select(o => new OrderResult()
			{
				OrderId = o.Id,
				Status = o.Status,
				Total = o.TotalAmount,
				CreatedAt = o.CreatedAt,
			}).ToListAsync();

			return orders;
		}

		public async Task<OrderDetailsResult> GetOrderDetails(Guid userId , Guid orderId)
		{
			if (userId==Guid.Empty)
			{
				throw new ArgumentException("user id cannot be empty",nameof(userId));
			}
			if (orderId == Guid.Empty)
			{
				throw new ArgumentException("order id cannot be empty", nameof(orderId));
			}
			var query = _orderRepository.GetOrderDetailsQuery(userId, orderId);
			OrderDetailsResult? order = await query.Select(o=>new OrderDetailsResult
			{
			  OrderId= o.Id,
			  Status = o.Status,
			  Total = o.TotalAmount,
			  CreatedAt = o.CreatedAt,
			  Address= new ShippingAddressDto
			  {
				  BuildingNumber=o.Address.BuildingNumber,
				  City=o.Address.City,
				  PostalCode=o.Address.PostalCode,
				  PhoneNumber=o.Address.PhoneNumber,
				  RecipientName=o.Address.RecipientName,
				  Street=o.Address.Street,
			  },
			  OrderItems= o.Items.Select(oi=>new OrderItemDto
			  {
				 ItemId= oi.Id,
				 LineTotal=oi.LineTotal,
				 ProductName=oi.ProductName,
				 Quantity=oi.Quantity,
				 UnitPrice=oi.UnitPrice,
			  }).ToList()		  
			}).FirstOrDefaultAsync();

			if (order==null)
			{
				throw new NotFoundException("Order not found");
			}
			return order;
		}
	}
}
