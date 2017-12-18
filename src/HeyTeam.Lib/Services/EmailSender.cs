using HeyTeam.Core.Services;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HeyTeam.Lib.Services {
	// This class is used by the application to send email for account confirmation and password reset.
	// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
	public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
			MailMessage m = new MailMessage();
			SmtpClient sc = new SmtpClient();
			m.From = new MailAddress("mapolafc@gmail.com");
			m.To.Add(email);
			m.Subject = subject;
			m.Body = message;
			m.IsBodyHtml = true;
			sc.UseDefaultCredentials = true;
			sc.Host = "in-v3.mailjet.com";
			sc.Port = 587;
			sc.Credentials = new System.Net.NetworkCredential("1cb053159d2432cf7f3f299485a6de12", "a73a05ef4a4272de4851704bc125efd4");
			sc.EnableSsl = true;
			try {
				await sc.SendMailAsync(m);
			}catch(Exception ex){

			}
        }
    }
}
