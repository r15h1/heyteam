using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core.Identity {
    public enum Roles {
        Administrator,
        Player,
        Coach
    }

    public static class RolesExtensions {
        public static bool Contains(this IEnumerable<string> roles, Roles role) {
            if(roles == null || roles.Count() == 0) 
                return false;

            return roles.Any(r => r.ToLowerInvariant().Equals(role.ToString().ToLowerInvariant()));
        }
    }
}