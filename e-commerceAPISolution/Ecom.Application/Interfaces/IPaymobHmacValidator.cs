using Ecom.Application.DTOs.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IPaymobHmacValidator
	{
		bool IsValid(PaymobWebhookObject obj , string receivedHmac);
	}
}
