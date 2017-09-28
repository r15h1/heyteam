using System;

namespace HeyTeam.Core.Exceptions {
    public class ClubNotFoundException : Exception {
        public ClubNotFoundException():base("The specified club was not found"){}
     }

    public class SquadNotFoundException : Exception { 
        public SquadNotFoundException():base("The specified squad was not found"){}
    }
}