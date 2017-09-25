using System;

namespace HeyTeam.Lib.Data {
    public class Club {
        public long? Id{ get; set; }
        public Guid Guid { get; private set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        //public List<Squad> Squads { get; private set; } = new List<Squad>();
    }
}