using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Web.Models.SquadViewModels {
    public class SquadDetailsViewModel {
        public string SquadName { get;set; }
        public string SquadId { get; set; }
        public IEnumerable<Player> Players { get;set; }
        
    }
}