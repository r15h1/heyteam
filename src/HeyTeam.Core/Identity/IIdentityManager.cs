using System.Collections.Generic;

namespace HeyTeam.Core.Identity {
    public interface IIdentityManager
    {
        void CreateUser(string user, string password);
        void AddUserToRole(string user, string role);
        IEnumerable<string> GetRoles(string user);
    }
}