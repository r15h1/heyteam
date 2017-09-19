using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (long? id) {
            this.Id = id;
        }
        
        public long? Id { get; private set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<Squad> Squads { get; private set; } = new List<Squad>();
    }
}