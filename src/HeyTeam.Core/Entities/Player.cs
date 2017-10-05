using System;
using HeyTeam.Util;

namespace HeyTeam.Core.Entities {
    public class Player {
        public Player (Guid squadId, Guid? playerId = null) {
            if(squadId.IsEmpty()) throw new ArgumentNullException();
            SquadId = squadId;
            Guid = playerId.HasValue && playerId.Value != Guid.Empty ? playerId.Value : Guid.NewGuid();
        }

        public Guid SquadId { get; }
        public Guid Guid { get; }
    }
}