using HeyTeam.Util;
using System;

namespace HeyTeam.Core {
	public class Squad {        
        public Squad(Guid clubId, Guid? squadId = null) {
            if(clubId.IsEmpty()) throw new ArgumentNullException();
            ClubId = clubId;
            Guid = squadId.HasValue && squadId.Value != Guid.Empty ? squadId.Value : Guid.NewGuid();
        }
        public Guid ClubId { get; }
        public Guid Guid { get; }
        public string Name { get; set; }
        public short YearBorn { get; set; }
        public int? NumberOfPlayers { get; set; }
    }
}