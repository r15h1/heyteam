using System.Collections.Generic;
using HeyTeam.Core.Identity;
using HeyTeam.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HeyTeam.Identity {
    public class IdentityManager : IIdentityManager {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityManager(UserManager<ApplicationUser> userManager) {
            this.userManager = userManager;
        }

        public async Task<IdentityOperationResult> SetupUser(Credential credential) {            
            var newUser = new ApplicationUser { UserName = credential.Email, Email = credential.Email };
            var result = await userManager.CreateAsync(newUser, credential.Password);
            var operationResult = new IdentityOperationResult(result.Succeeded);
            if(result.Succeeded)
                foreach(var role in credential.Roles)
                    await userManager.AddToRoleAsync(newUser, role.ToString().ToLowerInvariant());
            
            foreach(var e in result.Errors) operationResult.AddError(e.Description);

            return operationResult;
        }

        public IdentityOperationResult AddUserToRole(string email, string role) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetRoles(string email) {
            var user = userManager.FindByEmailAsync(email).Result;
            var roles = userManager.GetRolesAsync(user).Result;
            return roles;
        }
    }
}