using System.Collections.Generic;
using HeyTeam.Core.Identity;
using HeyTeam.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace HeyTeam.Identity {
    public class IdentityManager : IIdentityManager {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityManager(UserManager<ApplicationUser> userManager) {
            this.userManager = userManager;
        }

        public async Task<IdentityOperationResult> CreateUser(string email, string password) {            
            var newUser = new ApplicationUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(newUser, password);
            var operationResult = new IdentityOperationResult(result.Succeeded);
            foreach(var e in result.Errors) operationResult.AddError(e.Description);
            return operationResult;
        }

        public IdentityOperationResult AddUserToRole(string email, string role) {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetRoles(string email) {
            throw new System.NotImplementedException();
        }
    }
}