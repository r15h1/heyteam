using HeyTeam.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels {
	public class EventReviewViewModel
    {
		[Required(ErrorMessage = "EventId is required")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "SquadId is required")]
		public IEnumerable<Guid> Squads { get; set; }

		[Required(ErrorMessage = "CoachId is required")]
		public Guid CoachId { get; set; }

		public DateTime? LastReviewDate { get; set; }

		public IEnumerable<Squad> SquadsNotYetReviewed { get; set; }
		public string EventTitle { get; set; }
		public string EventDetails { get; set; }
		public IEnumerable<EventReview> Reviews { get; set; }
	}
}