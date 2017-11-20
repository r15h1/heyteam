using System.Collections.Generic;
using HeyTeam.Core.Dashboard;

namespace HeyTeam.Web.Models.DashboardViewModels {
    public class IndexViewModel {
        public IEnumerable<string> Roles { get; set; }
        public ICollection<Item> Squads { get; set; }
		public IEnumerable<string> Errors { get; set; }
    }
}