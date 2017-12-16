using HeyTeam.Util;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeyTeam.Identity {
	public class RolesHandler : AuthorizationHandler<RolesRequirement> {
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement) {
			if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role)) {				
				return Task.CompletedTask;
			}

			if (context.User.Claims.Any(c => c.Type == ClaimTypes.Role && !c.Value.IsEmpty() && requirement.Roles.Contains(c.Value)))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}