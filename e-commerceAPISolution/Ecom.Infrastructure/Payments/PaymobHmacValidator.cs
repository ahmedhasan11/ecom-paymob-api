using Ecom.Application.DTOs.Webhooks;
using Ecom.Application.Interfaces;
using Ecom.Infrastructure.Common.Settings;
using Hangfire.Logging;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger<PaymobHmacValidator> _logger;
		public PaymobHmacValidator(IOptions<PaymobSettings> paymobSettings, ILogger<PaymobHmacValidator> logger)
		{
			_paymobSettings = paymobSettings.Value;
			_logger = logger;
		}
		public bool IsValid(PaymobWebhookObject obj, string receivedHmac)
		{
			// 1. Null or empty check
			if (string.IsNullOrWhiteSpace(receivedHmac))
			{
				_logger.LogWarning("Received HMAC is null or empty.");
				return false;
			}
			// 2. Length validation (SHA512 produces 64 bytes = 128 hex characters)
			if (receivedHmac.Length != 128)
			{
				_logger.LogWarning("Paymob HMAC validation failed: Invalid HMAC length ({Length}). Expected 128.", receivedHmac.Length);
				return false;
			}

			var hmacSecret = _paymobSettings.HmacSecret;
			//if there was a misconfiguration in the HmacSecret in the appsettings or it was empty or null
			if (string.IsNullOrWhiteSpace(hmacSecret))
			{
				_logger.LogCritical("Paymob HMAC validation failed: HmacSecret is not configured.");
				return false;
			}

			byte[] receivedHmacBytes;

			// ✅ safe parsing (the only important fix)
			try
			{
				receivedHmacBytes = Convert.FromHexString(receivedHmac);
			}
			catch (FormatException)
			{
				_logger.LogWarning("Invalid HMAC format (not hex).");
				return false;
			}



			var hmacBytes= Encoding.UTF8.GetBytes(hmacSecret); //7wl el text(raw string) l bytes [3shan kda UTF8]
			using var hmac = new HMACSHA512(hmacBytes);
			var concatenatedString = string.Concat(obj.AmountCents, obj.CreatedAt ?? "", obj.Currency ?? ""
			, obj.ErrorOccured.ToString().ToLowerInvariant(), obj.HasParentTransaction.ToString().ToLowerInvariant(), obj.TransactionId, obj.IntegrationId
			, obj.Is3DSecure.ToString().ToLowerInvariant(), obj.IsAuth.ToString().ToLowerInvariant(), obj.IsCapture.ToString().ToLowerInvariant(), obj.IsRefunded.ToString().ToLowerInvariant()
			, obj.IsStandalonePayment.ToString().ToLowerInvariant(), obj.IsVoided.ToString().ToLowerInvariant(), obj.Order?.Id ?? 0, obj.Owner
			, obj.Pending.ToString().ToLowerInvariant(), obj.SourceData?.Pan ?? "", obj.SourceData?.SubType ?? "", obj.SourceData?.Type ?? ""
			, obj.Success.ToString().ToLowerInvariant());
			
			var dataBytes = Encoding.UTF8.GetBytes(concatenatedString);
			var computedHash = hmac.ComputeHash(dataBytes); //computed bytes 


			var isValid = CryptographicOperations.FixedTimeEquals(computedHash, receivedHmacBytes);
			if (!isValid)
			{
				return false;
			}
			return true; 
		}
	}
}
