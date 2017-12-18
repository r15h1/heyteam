using System.Threading.Tasks;

namespace HeyTeam.Core.Services {
	public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
