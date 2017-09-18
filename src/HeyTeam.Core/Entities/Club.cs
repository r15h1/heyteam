using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            CheckPolicies(squad);
            Squads.Add(squad);
        }
        private void CheckPolicies(Squad squad)
        {
            var policyViolations = new StringBuilder();

            if (!Id.HasValue) policyViolations.AppendLine("Squads cannot be added to unregistered clubs");
            if (squad.Club.Id != Id) policyViolations.AppendLine("Squad must belong to the same club");
            if(!squad.Id.HasValue)                        
            {
                bool sameNameExists = Squads.FirstOrDefault(s => s.Name.ToLowerInvariant().Trim().Equals(squad.Name.ToLowerInvariant().Trim())) != null;
                if(sameNameExists) policyViolations.AppendLine("A squad with the same name exists already");
            }

            if (policyViolations.Length > 0) throw new PolicyViolationException(policyViolations.ToString());
        }
    }
}