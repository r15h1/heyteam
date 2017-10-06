using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Exceptions;
using HeyTeam.Util;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (Guid? guid = null) {
            Guid = guid.HasValue ? guid.Value : System.Guid.NewGuid();
        }
        public Guid Guid { get; private set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<Squad> Squads { get; } = new List<Squad>();
        public void AddSquad(Squad squad) {
            Ensure.ArgumentNotNull(squad);
            if(Squads.Any(s => s.Guid == squad.Guid)) throw new DuplicateEntryException("A squad with this id exists already");
            if(Squads.Any(s => s.Name.ToLowerInvariant().Equals(squad.Name.ToLowerInvariant()))) throw new DuplicateEntryException("A squad with this name exists already");

            Squads.Add(squad);
        }

        public void UpdateSquad(Squad squad) {
            Ensure.ArgumentNotNull(squad);
            if(squad.ClubId != Guid) 
                throw new InvalidOperationException("Target squad belongs to a different club");

            if(!Squads.Any(s => s.Guid == squad.Guid))
                throw new SquadNotFoundException();

            if(Squads.Any(s => s.Guid != squad.Guid && s.Name.ToLowerInvariant().Equals(squad.Name.ToLowerInvariant()))) 
                throw new DuplicateEntryException("Another squad already has this name");

            var sq = Squads.FirstOrDefault(s => s.Guid == squad.Guid);
            sq.Name = squad.Name;
        }
    }
}