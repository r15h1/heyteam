using System.Collections.Generic;

namespace HeyTeam.Core.Repositories {
    public interface IUserRepository
    {
        IEnumerable<string> GetRoles(string userEmail);
    }
}