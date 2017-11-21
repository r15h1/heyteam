using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.EventsViewModels {
    public class EventListViewModel {		
		public Guid EventId { get; set; }
		public string Title{ get; set; }
		public  string Squads { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Location { get; set; }
	}
}