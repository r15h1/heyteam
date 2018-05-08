using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeyTeam.Core.Identity {
    public interface IIdentityManager
    {
		Task<IdentityOperationResult> SetupUser(Credential credential);
        IdentityOperationResult AddUserToRole(string email, string role);
		Task<IdentityOperationResult> ToggleLock(string email);
		Task<IdentityOperationResult> RemoveUser(string email);
		IEnumerable<string> GetRoles(string email);
		Task<IdentityOperationResult> RemoveUserRole(string email, Membership membership);
	}
}