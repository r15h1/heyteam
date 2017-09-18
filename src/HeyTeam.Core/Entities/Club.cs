using System;
using System.Collections.Generic;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Core.Entities {
    public class Club {
        public Club (long? id) {
            this.Id = id;
        }
        public long? Id { get; private set; }        
        public string Name { get; set; }

        public List<Squad> Squads { get; private set; } = new List<Squad>();
        public void AddSquad(Squad squad) {
            Validate(squad);
            Squads.Add(squad);
        }
        private void Validate(Squad squad)
        {
            if (!Id.HasValue) throw new IllegalOperationException("Squads cannot be added to unregistered clubs");
            if (squad == null) throw new ArgumentNullException();
            if (string.IsNullOrWhiteSpace(squad.Name)) throw new IllegalOperationException ("Squad name cannot be empty"); 
        }
    }
}