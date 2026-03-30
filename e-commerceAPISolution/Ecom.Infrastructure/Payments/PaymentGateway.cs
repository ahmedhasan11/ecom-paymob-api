using Ecom.Application.Common.Settings;
using Ecom.Application.DTOs.Payments;
using Ecom.Application.Interfaces;
using Ecom.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Payments
{
	public class PaymentGateway : IPaymentGateway
	{
		private readonly HttpClient _httpClient;
		private readonly PaymobSettings _paymob;	
		public PaymentGateway(IOptions<PaymobSettings> paymob, HttpClient httpClient) 
		{
			_paymob = paymob.Value;
			_httpClient = httpClient;
		}
		public async Task<PaymentSessionResult> CreatePaymentSessionAsync(CreatePaymentSessionRequest req,  CancellationToken cancellationToken)
		{
			var amountInCents = (int)(req.Amount * 100); //Amount
			List<PaymobItem> items = req.Items.Select(item => new PaymobItem
			{
				name=item.Name,
				quantity=item.Quantity,
			    amount = (int)(item.UnitPrice*100),
			    description=item.Description,
			}).ToList(); //Items
			PaymobBillingData billingData = new PaymobBillingData()
			{
			   first_name= req.BillingData.FirstName,
			   last_name= req.BillingData.LastName,
			   street=req.BillingData.Street,
			   city=req.BillingData.City,
			   country=req.BillingData.Country,
			   email=req.BillingData.Email,
			   phone_number=req.BillingData.PhoneNumber,
			}; //Billing Data
			PaymobCreateIntentionRequest paymobRequest = new PaymobCreateIntentionRequest
			{
			   amount = amountInCents,
			   items = items,
			   currency = req.Currency,
			   merchant_order_id = req.PaymentId.ToString(),
			   payment_methods = [ _paymob.IntegrationId ],
			   special_reference= req.OrderId.ToString(),
			   billing_data=billingData
			}; //Request

			//var content = JsonSerializer.Serialize(paymobRequest);
			var content = JsonContent.Create(paymobRequest); //make serializing + set content type = application/json   // convert obj --> JSON
			var response = await _httpClient.PostAsync("/v1/intention", content, cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync(cancellationToken);
				throw new Exception($"Paymob intention creation failed: {error}");
			}

			PaymobCreateIntentionResponse? paymobResponse = await response.Content.ReadFromJsonAsync<PaymobCreateIntentionResponse>(cancellationToken);
			if (paymobResponse is null)
			{
				throw new Exception("Paymob returned empty response");
			}
		}
	}
}
