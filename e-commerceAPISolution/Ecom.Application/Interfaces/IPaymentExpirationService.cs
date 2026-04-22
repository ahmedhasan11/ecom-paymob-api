using Ecom.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IPaymentExpirationService
	{
		 Task ExpirePaymentsAsync(CancellationToken cancellationToken);
	}
}
