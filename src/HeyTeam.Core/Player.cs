using System;
using System.Collections.Generic;
using HeyTeam.Util;

namespace HeyTeam.Core {
    public class Player : Member {
        public Player (Guid squadId, Guid? playerId = null) : base(playerId){
            if(squadId.IsEmpty()) throw new ArgumentNullException();
            SquadId = squadId;
			Squads.Add(squadId);
        }

        public Guid SquadId { get; }        
        public char DominantFoot { get; set; }
        public string Nationality { get; set; }
        public List<string> Positions { get; set; }
        public short? SquadNumber { get; set; }        
    }
}