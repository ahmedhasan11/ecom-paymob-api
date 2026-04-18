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
		//start hmac order
		//amount_cents
		[JsonPropertyName("amount_cents")]
		public int AmountCents { get; set; }
		//created_at
		[JsonPropertyName("created_at")]
		public string? CreatedAt { get; set; }
		//currency
		[JsonPropertyName("currency")]
		public string? Currency { get; set; }
		//error_occured
		[JsonPropertyName("error_occured")]
		public bool ErrorOccured { get; set; }
		//has_parent_transaction
		[JsonPropertyName("has_parent_transaction")]
		public bool HasParentTransaction { get; set; }
		//obj.id
		[JsonPropertyName("id")]
		public long TransactionId { get; set; }
		//integration_id
		[JsonPropertyName("integration_id")]
		public int IntegrationId { get; set; }
		//is_3d_secure
		[JsonPropertyName("is_3d_secure")]
		public bool Is3DSecure { get; set; }
		//is_auth
		[JsonPropertyName("is_auth")]
		public bool IsAuth { get; set; }
		//is_capture
		[JsonPropertyName("is_capture")]
		public bool IsCapture { get; set; }
		//is_refunded
		[JsonPropertyName("is_refunded")]
		public bool IsRefunded { get; set; }
		//is_standalone_payment
		[JsonPropertyName("is_standalone_payment")]
		public bool IsStandalonePayment { get; set; }
		//is_voided
		[JsonPropertyName("is_voided")]
		public bool IsVoided { get; set; }
		//order.id 
		[JsonPropertyName("order")]
		public PaymobWebhookOrder? Order { get; set; }
		//owner
		[JsonPropertyName("owner")]
		public int Owner { get; set; }
		//pending
		[JsonPropertyName("pending")]
		public bool Pending { get; set; }

		//source_data.pan
		//source_data.sub_type
		//source_data.type
		[JsonPropertyName("source_data")]
		public PaymobSourceData? SourceData { get; set; }
		//success
		[JsonPropertyName("success")]
		public bool Success { get; set; }

	}
}
