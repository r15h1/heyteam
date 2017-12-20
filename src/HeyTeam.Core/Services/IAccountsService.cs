using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface IAccountsService 
	{
		Response SendInvitation(AccountRequest request);
		Response ToggleLock(AccountRequest request);
		Response CreateMemberAccount(MembershipRequest request);
		(Response Response, Invitation Invitation) VerifyToken(TokenVerificationRequest request);
    }

	public class AccountRequest {
		public Guid ClubId { get; set; }
		public string Email { get; set; }
	}

	public class MembershipRequest {
		public string Token { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class TokenVerificationRequest{
		public string Token { get; set; }
		public Guid ClubId{ get; set; }
	}

	public class Invitation {
		public Guid? ClubId { get; set; }
		public string Email { get; set; }
		public DateTime? Expiry { get; set; }
	}
}