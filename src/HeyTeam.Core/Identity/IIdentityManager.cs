using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeyTeam.Core.Identity {
    public interface IIdentityManager
    {
        Task<IdentityOperationResult> CreateUser(string email, string password);
        IdentityOperationResult AddUserToRole(string email, string role);
        IEnumerable<string> GetRoles(string email);
    }
}