using Ecom.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Order
{
	public class OrderStatusRequestDto
	{
		public OrderStatusEnum Status { get; set; }
	}
}
