using HeyTeam.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Areas.Coaches.Models
{
    public class EventDetailsViewModel
    {
		public List<SelectListItem> SquadList { get; set; } = new List<SelectListItem>();
		
		public string Title { get; set; }
		public IEnumerable<Guid> Squads { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string Location { get; set; }

		public Guid? EventId { get; set; }

		public IEnumerable<TrainingMaterial> TrainingMaterials { get; set; }
		
	}
}
