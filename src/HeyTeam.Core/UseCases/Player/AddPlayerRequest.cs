using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases.Player {
    public class AddPlayerRequest
    {
        public Guid SquadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char DominantFoot { get; set; }
        public string Nationality { get; set; }
        public short? SquadNumber { get; set; }
        public List<string> Positions { get; set; } = new List<string>();
    }
}