using HeyTeam.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HeyTeam.Web.Areas.Coaches.Models {
	public class EventsViewModel
    {
		public IEnumerable<EventSummary> EventSummary { get; set; }
		public List<SelectListItem> Squads { get; set; }
	}
}
