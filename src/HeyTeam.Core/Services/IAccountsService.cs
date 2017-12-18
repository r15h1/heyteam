using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Services
{
    public interface IAccountsService 
	{
		Response SendInvitation(InvitationRequest request);
		Response CreateMemberAccount(MembershipRequest request);
    }

	public class InvitationRequest {
		public Guid ClubId { get; set; }
		public string Email { get; set; }
	}

	public class MembershipRequest {
		public string Token { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}