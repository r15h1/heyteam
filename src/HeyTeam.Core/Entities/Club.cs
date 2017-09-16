using System;
using System.Collections.Generic;
using HeyTeam.Core.Exceptions;

namespace HeyTeam.Core.Entities {
    public class Club {
        public List<Squad> Squads { get; private set; } = new List<Squad>();

        public void AddSquad(Squad squad) {
            Validate(squad);
            Squads.Add(squad);
        }

        private void Validate(Squad squad)
        {
            if (squad == null) throw new ArgumentNullException();
            if (string.IsNullOrWhiteSpace(squad.Name)) throw new IllegalOperationException ("Squad name cannot be empty"); 
        }
    }
}