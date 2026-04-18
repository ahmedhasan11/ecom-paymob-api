using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Interfaces;
using Ecom.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymobHmacValidator : IPaymobHmacValidator
	{
		private readonly PaymobSettings _paymobSettings;
		public PaymobHmacValidator(IOptions<PaymobSettings> paymobSettings)
		{
			_paymobSettings = paymobSettings.Value;
		}
		public bool IsValid(PaymobWebhookObject obj, string receivedHmac)
		{	
			var hmacSecret = _paymobSettings.HmacSecret;
			var hmacBytes= Encoding.UTF8.GetBytes(hmacSecret);
			using var hmac = new HMACSHA512(hmacBytes);
			var concatenatedString = string.Concat(obj.AmountCents, obj.CreatedAt ?? "", obj.Currency ?? ""
			, obj.ErrorOccured.ToString().ToLower(), obj.HasParentTransaction.ToString().ToLower(), obj.TransactionId, obj.IntegrationId
			, obj.Is3DSecure.ToString().ToLower(), obj.IsAuth.ToString().ToLower(), obj.IsCapture.ToString().ToLower(), obj.IsRefunded.ToString().ToLower()
			, obj.IsStandalonePayment.ToString().ToLower(), obj.IsVoided.ToString().ToLower(), obj.Order?.Id ?? 0, obj.Owner
			, obj.Pending.ToString().ToLower(), obj.SourceData?.Pan ?? "", obj.SourceData?.SubType ?? "", obj.SourceData?.Type ?? ""
			, obj.Success.ToString().ToLower());
			
			var dataBytes = Encoding.UTF8.GetBytes(concatenatedString);
			var hash = hmac.ComputeHash(dataBytes);
			var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

			if (result!=receivedHmac)
			{
				return false;
			}
			return true; 
		}
	}
}
