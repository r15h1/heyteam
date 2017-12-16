using Microsoft.AspNetCore.Authorization;

namespace HeyTeam.Identity {
	public class RolesRequirement : IAuthorizationRequirement {
		public RolesRequirement(string[] roles) {
			this.Roles = roles;
		}

		public string[] Roles { get; }
	}
}
