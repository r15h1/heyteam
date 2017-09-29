using System;
using HeyTeam.Util;

namespace HeyTeam.Core.Entities {
    public class Squad {
        public Squad(Guid clubId, Guid? squadId = null) {
            if(clubId.IsEmpty()) throw new ArgumentNullException();
            ClubId = clubId;
            Guid = squadId.HasValue && squadId.Value != Guid.Empty ? squadId.Value : Guid.NewGuid();
        }
        public Guid ClubId { get; }
        public Guid Guid { get; }
        public string Name { get; set; }
    }
}