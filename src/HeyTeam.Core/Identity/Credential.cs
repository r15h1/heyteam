using System.Collections.Generic;

namespace  HeyTeam.Core.Identity {
    public class Credential {
        public string Email { get ;set; }
        public string Password { get ;set; }
        public List<Roles> Roles { get ;set; } = new List<Roles>();
    }
}