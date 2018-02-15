using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HeyTeam.Web.Services;
using HeyTeam.Core.Services;

namespace HeyTeam.Web.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
			var request = new EmailRequest { Subject = "Confirm your email", Message = $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>" };
			request.AddEmailAddress(email, Recipient.To);
            return emailSender.EmailAsync(request);
        }
    }
}
