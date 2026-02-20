using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.Exceptions
{
	public class EmailSendingException: Exception
	{
		public EmailSendingException(string message, Exception inner) : base(message, inner) { }
	}
}
