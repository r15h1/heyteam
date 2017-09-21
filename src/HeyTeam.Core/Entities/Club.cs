using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (Guid? id = null) {
            Id = id.HasValue ? id.Value : System.Guid.NewGuid();
        }
        
        public Guid Id { get; private set; }        
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<Squad> Squads { get; private set; } = new List<Squad>();
        public void AddSquad(Squad squad) {
            if(Squads.Any(s => s.Id == squad.Id)) throw new DuplicateEntryException("A squad with this id exists already");
            if(Squads.Any(s => s.Name.ToLowerInvariant().Equals(squad.Name.ToLowerInvariant()))) throw new DuplicateEntryException("A squad with this name exists already");

            Squads.Add(squad);
        }
    }
}