using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Interfaces
{
	public interface IPaymentConfiguration
	{
		public int ExpirationSeconds { get; }
	}
}
