using System;
using System.Threading.Tasks;
using HeyTeam.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeyTeam.Web.Services {
	public class MembershipRequirement : IAuthorizationRequirement { 
		public MembershipRequirement(string membership) {
			Membership = membership;
		}

		public string Membership { get; }
	}

	public class MembershipRequirementHandler : AuthorizationHandler<MembershipRequirement> {
		private readonly IMemberQuery memberQuery;

		public MembershipRequirementHandler(IMemberQuery memberQuery) {
			this.memberQuery = memberQuery;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MembershipRequirement requirement) {
			if (context.Resource is AuthorizationFilterContext mvcContext) {
				// Examine MVC-specific things like routing data.
				if (mvcContext.RouteData.Values.ContainsKey("memberid")) {
					string memberid = mvcContext.RouteData.Values["memberid"] as string;
					var member = memberQuery.GetPlayer(Guid.Parse(memberid));
					if (member != null)
						context.Succeed(requirement);
				}
			}
			return Task.CompletedTask;
		}
	}
}
