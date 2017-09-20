using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (Guid? id = null) {
            Id = id.HasValue ? id.Value : System.Guid.NewGuid();
        }
        
        public Guid Id { get; private set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<Squad> Squads { get; private set; } = new List<Squad>();
    }
}