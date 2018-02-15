using HeyTeam.Core.Models;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HeyTeam.Lib.Services {
	// This class is used by the application to send email for account confirmation and password reset.
	// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
	public class EmailSender : IEmailSender
    {
		private SmtpSettings smtpSettings;

		public EmailSender(IOptions<SmtpConfiguration> configuration){
			this.smtpSettings = configuration.Value.SmtpSettings;
		}

		public async Task EmailAsync(EmailRequest request) => await SendEmailAsync(request);

		private async Task SendEmailAsync(EmailRequest request){
			if (request == null) 
				throw new ArgumentNullException(nameof(request));

			MailMessage message = new MailMessage() {
				Subject = request.Subject,
				Body = request.Message,
				From = new MailAddress(smtpSettings.FromEmailAddress),
				IsBodyHtml = true
			};

			SmtpClient client = new SmtpClient() {
				UseDefaultCredentials = true,
				Host = smtpSettings.Host,
				Port = smtpSettings.Port,
				Credentials = new System.Net.NetworkCredential(smtpSettings.Username, smtpSettings.Password),
				EnableSsl = true
			};

			foreach(var address in request.To) message.To.Add(address);
			foreach (var address in request.CC) message.CC.Add(address);
			foreach (var address in request.BCC) message.Bcc.Add(address);

			try {
				await client.SendMailAsync(message);
			} catch (Exception ex) {}
		}
	}
}
