using System.Collections.Generic;
using HeyTeam.Core.Identity;
using HeyTeam.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace HeyTeam.Identity {
    public class IdentityManager : IIdentityManager
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public void CreateUser(string user, string password)
        {
            throw new System.NotImplementedException();
        }

        public void AddUserToRole(string user, string role)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetRoles(string user)
        {
            throw new System.NotImplementedException();
        }
    }
}