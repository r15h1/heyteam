using System;
using System.Collections.Generic;
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
        public DateTime DateOfBirth { get; set; }
        public char DominantFoot { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public List<string> Positions { get; set; }
        public short? SquadNumber { get; set; }
        public string Email { get; set; }
    }
}