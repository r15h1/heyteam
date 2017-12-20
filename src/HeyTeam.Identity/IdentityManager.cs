using HeyTeam.Core;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Queries;
using HeyTeam.Util;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Identity {
	public class IdentityManager : IIdentityManager {
		private readonly Club club;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IMemberQuery memberQuery;

		public IdentityManager(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) {
			this.club = club;
			this.userManager = userManager;
			this.memberQuery = memberQuery;
		}

        public async Task<IdentityOperationResult> SetupUser(Credential credential) {
			var roles = memberQuery.GetMembersByEmail(club.Guid, credential.Email).Select(m => m.Membership).Distinct();
			var newUser = new ApplicationUser { UserName = credential.Email, Email = credential.Email };
			
			try {
				var result = await userManager.CreateAsync(newUser, credential.Password);
				if (result.Succeeded) {
					foreach (var role in roles) {
						var roleResult = await userManager.AddToRoleAsync(newUser, role.ToString().ToLowerInvariant());
						if (!roleResult.Succeeded) {
							var operationResult = new IdentityOperationResult(false);
							foreach (var e in result.Errors) 
								operationResult.AddError(e.Description);

							return operationResult;
						}							
					}			
					return new IdentityOperationResult(true);
				} else {
					var operationResult = new IdentityOperationResult(false);
					foreach (var e in result.Errors) 
						operationResult.AddError(e.Description);

					return operationResult;
				}
			}catch(Exception ex) {
				var operationResult = new IdentityOperationResult(false);
				operationResult.AddError(ex.Message);
				return operationResult;
			}			
        }

        public IdentityOperationResult AddUserToRole(string email, string role) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetRoles(string email) {
            var user = userManager.FindByEmailAsync(email).Result;
            var roles = userManager.GetRolesAsync(user).Result;
            return roles;
        }

		public async Task<IdentityOperationResult> ToggleLock(string email) {
			var user = await userManager.FindByEmailAsync(email);
			if (user == null) 
				return GetResult(false, new string[] { "The specified user does not exist" });

			var isLockedOut = await userManager.IsLockedOutAsync(user);
			await userManager.SetLockoutEndDateAsync(user, (isLockedOut ? null : DateTimeOffset.MaxValue as DateTimeOffset?));
			return new IdentityOperationResult(true);
		}

		public IdentityOperationResult GetResult(bool success, IEnumerable<string> messages = null) {
			var result = new IdentityOperationResult(success);
			if (messages != null && messages.Any())
				foreach(var message in messages)
					result.AddError(message);

			return result;
		}
	}
}