using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HeyTeam.Web.Services {
	// This class is used by the application to send email for account confirmation and password reset.
	// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
	public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
			MailMessage m = new MailMessage();
			SmtpClient sc = new SmtpClient();
			m.From = new MailAddress("my@gmail.com");
			m.To.Add(email);
			m.Subject = "This is a test";
			m.Body = "This is a sample message using SMTP authentication";
			sc.UseDefaultCredentials = true;
			sc.Host = "smtp.gmail.com";
			sc.Port = 587;
			sc.Credentials = new System.Net.NetworkCredential("my@gmail.com", "hahahjjkjhasjdhkasjdhkasjdlahskjdahskdjh");
			sc.EnableSsl = true;
			try {
				await sc.SendMailAsync(m);
			}catch(Exception ex){

			}
        }
    }
}
