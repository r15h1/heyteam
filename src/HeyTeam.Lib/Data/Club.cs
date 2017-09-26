using System;
using System.Collections.Generic;

namespace HeyTeam.Lib.Data {
    public class Club {
        public long? ClubId{ get; set; }
        public Guid Guid { get; set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public ICollection<Squad> Squads { get; set; }
    }
}