using HeyTeam.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeyTeam.Util;
using System;

namespace HeyTeam.Core.Services {
	public interface IEmailSender
    {
        Task EmailAsync(EmailRequest request);
    }

	public class EmailRequest{
		private delegate bool AddEmail(string emailAddress);

		public HashSet<string> To { get; } = new HashSet<string>();
		public HashSet<string> CC { get; } = new HashSet<string>();
		public HashSet<string> BCC { get; } = new HashSet<string>();

		public void AddEmailAddress(string emailAddress, Recipient recipient) {
			if (emailAddress.IsValidEmail()) {				
				AddEmail del = GetRecipientDelegate(recipient);
				del(emailAddress);
			}			
		}

		private AddEmail GetRecipientDelegate(Recipient recipient) {
			if (recipient == Recipient.To) return To.Add;
			if (recipient == Recipient.CC) return CC.Add;

			return BCC.Add;				
		}

		public string Subject{ get; set; }

		public string Message { get; set; }
	}

	public enum Recipient{
		To,
		CC,
		BCC
	}
}
