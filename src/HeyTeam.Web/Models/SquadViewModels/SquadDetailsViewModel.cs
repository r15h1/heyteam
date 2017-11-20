using HeyTeam.Core;
using System.Collections.Generic;

namespace HeyTeam.Web.Models.SquadViewModels {
	public class SquadDetailsViewModel {
        public string SquadName { get;set; }
        public string SquadId { get; set; }
        public IEnumerable<Player> Players { get;set; }
		public Coach Coach { get; set; }        
    }
}