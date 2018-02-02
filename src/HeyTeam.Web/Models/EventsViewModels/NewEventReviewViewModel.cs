using HeyTeam.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Models.EventsViewModels
{
    public class NewEventReviewViewModel
    {
		[Required(ErrorMessage = "EventId is required")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "At least one squad is required")]
		[MinLength(1)]
		public IEnumerable<Guid> Squads { get; set; }

		public List<SelectListItem> SquadsNotYetReviewed { get; set; }

		[Required(ErrorMessage = "This field is required")]
		public string Successes { get; set; }//What went well

		[Required(ErrorMessage = "This field is required")]
		public string Opportunities { get; set; }//What did not go well

		[Required(ErrorMessage = "This field is required")]
		public string DifferentNextTime { get; set; }//What could have been done differently

		public string EventTitle { get; set; }
		public string EventDetails { get; set; }
		public Guid MemberId { get; set; }
	}
}
