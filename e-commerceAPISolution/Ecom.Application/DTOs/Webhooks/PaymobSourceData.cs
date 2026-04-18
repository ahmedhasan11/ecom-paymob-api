using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecom.Application.DTOs.Webhooks
{
	public class PaymobSourceData
	{
		//source_data.pan
		[JsonPropertyName("pan")]
		public string? Pan { get; set; }
		//source_data.sub_type
		[JsonPropertyName("sub_type")]
		public string? SubType { get; set; }
		//source_data.type
		[JsonPropertyName("type")]
		public string? Type { get; set; }



	}
}
