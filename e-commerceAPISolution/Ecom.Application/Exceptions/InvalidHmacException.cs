using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Exceptions
{
	public class InvalidHmacException:Exception
	{
		public InvalidHmacException(string message) : base(message) { }
	}
}
