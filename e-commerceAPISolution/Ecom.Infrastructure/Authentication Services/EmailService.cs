using Ecom.Application.Common.Settings;
using Ecom.Application.Exceptions;
using Ecom.Application.Interfaces;
using Microsoft.Extensions.Options;
using Pipelines.Sockets.Unofficial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
		public async Task SendEmailAsync(string to, string subject, string body)
		{
			using var message = new MailMessage();
			message.From = new MailAddress(_emailSettings.Username);
			message.To.Add(to);
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = true;

			using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
			smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
			smtp.EnableSsl = true;
			try{
				await smtp.SendMailAsync(message);
			}
			catch (Exception ex)
			{
				throw new EmailSendingException("Failed to send email.", ex);
			}
			
		}
	}
}
