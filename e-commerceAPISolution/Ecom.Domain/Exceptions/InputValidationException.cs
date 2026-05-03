using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Domain.Exceptions
{
	public class InputValidationException: Exception
	{
		public InputValidationException(string message) : base(message) { }
	}
}
