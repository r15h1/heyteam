using HeyTeam.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HeyTeam.Web.Areas.Coaches.Models {
	public class EventsViewModel
    {
		public List<SelectListItem> Squads { get; set; }
	}
}
