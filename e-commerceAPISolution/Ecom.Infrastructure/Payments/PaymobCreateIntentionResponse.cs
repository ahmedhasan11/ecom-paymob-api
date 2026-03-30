using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymobCreateIntentionResponse
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = default!; //IntentionId

		[JsonPropertyName("client_secret")]
		public string ClientSecret { get; set; } = default!;

		[JsonPropertyName("intention_order_id")]
		public long PaymobOrderId { get; set; } 

		[JsonPropertyName("amount")]
		public int Amount { get; set; }

		[JsonPropertyName("currency")]
		public string Currency { get; set; } = default!;

		[JsonPropertyName("status")]
		public string Status { get; set; } = default!;

		[JsonPropertyName("created")]
		public DateTime CreatedAt { get; set; }

		[JsonPropertyName("created")]
		public int Expiration { get; set; }


	}
}
