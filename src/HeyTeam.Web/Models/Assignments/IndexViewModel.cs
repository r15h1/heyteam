using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace HeyTeam.Web.Models.Assignments {
	public class IndexViewModel
    {
		public List<SelectListItem> Squads{ get; set; }
		public Guid PlayerId { get; set; }
    }
}
