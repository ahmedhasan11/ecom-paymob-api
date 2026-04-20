using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Webhooks
{
	public class PaymentWebhookRequest
	{
		[JsonPropertyName("obj")]
		public PaymobWebhookObject? Obj { get; set; } 
	}
}
