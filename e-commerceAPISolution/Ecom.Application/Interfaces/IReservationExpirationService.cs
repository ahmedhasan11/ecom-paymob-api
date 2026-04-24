using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IReservationExpirationService
	{
		Task ExpireReservationsAsync(CancellationToken cancellationToken);
	}
}
