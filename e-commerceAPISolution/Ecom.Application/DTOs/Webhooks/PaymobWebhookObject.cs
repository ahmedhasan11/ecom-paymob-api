using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Webhooks
{
	public class PaymobWebhookObject
	{
		[JsonPropertyName("id")]
		public long TransactionId { get; set; }

		[JsonPropertyName("pending")]
		public bool Pending { get; set; }

		[JsonPropertyName("success")]
		public bool Success { get; set; }
		[JsonPropertyName("order")]
		public PaymobWebhookOrder Order { get; set; } = default!;
	}
}
