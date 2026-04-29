using Ecom.Application.Common.Pagination;
using Ecom.Application.DTOs.Order;
using Ecom.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_commerceAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly ICurrentUserService _currentUserService;

		public OrderController(IOrderService orderService, ICurrentUserService currentUserService) 
		{ 
			_orderService = orderService; 
			_currentUserService = currentUserService;
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPatch("{orderId}/status")]
		public async Task<ActionResult<OrderResult>> UpdateOrderStatus(Guid orderId,OrderStatusRequestDto dto,CancellationToken cancellationToken)
		{
			if (orderId==Guid.Empty)
			{
				return BadRequest("Order Id cannot be empty.");
			}
			OrderResult result = await _orderService.UpdateOrderStatusAsync(orderId, dto,cancellationToken);
			return Ok(result);
		}

		[HttpGet("my")]
		public async Task<ActionResult<PagedResult<OrderResult>>> GetUserOrders(OrdersPaginationOptions dtoOptions, CancellationToken cancellationToken)
		{
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			var orders= await _orderService.GetUserOrdersSummaryAsync(userId.Value, dtoOptions, cancellationToken);
			return Ok(orders);
		}

		[HttpGet("{orderId}")]
		public async Task<ActionResult<OrderDetailsResult>> GetOrderDetails(Guid orderId, CancellationToken cancellationToken)
		{
			if (orderId==Guid.Empty)
			{
				return BadRequest("Order Id cannot be empty.");
			}
			var userId = _currentUserService.UserId;
			if (userId == null)
			{
				return Unauthorized();
			}
			var order = await _orderService.GetOrderDetails(userId.Value, orderId, cancellationToken);
			return Ok(order);
		}
	}
}
