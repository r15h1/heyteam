using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Exceptions;
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
        public List<Player> Players {get;} = new List<Player>();
        public void AddPlayer(Player player)
        {
            Ensure.ArgumentNotNull(player);
            if(Players.Any(p => p.Guid == player.Guid)) throw new DuplicateEntryException("A player with this id exists already");

            //business rule for squad number tbd maybe auto-generated
            //unique at club level?
        }
    }
}