using Ecom.Application.Common.Settings;
using Ecom.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Authentication_Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;
		public EmailService( IOptions<EmailSettings> emailsettings) 
		{
		_emailSettings=emailsettings.Value;
		}
		public Task SendEmailAsync(string to, string subject, string body)
		{
			throw new NotImplementedException();
		}
	}
}
