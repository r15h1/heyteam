using HeyTeam.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels
{
    public class EventFeedbackViewModel {

		[Required(ErrorMessage = "EventId is required")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "SquadId is required")]
		public Guid SquadId { get; set; }

		[Required(ErrorMessage = "PlayerId is required")]
		public Guid PlayerId { get; set; }

		public string Feedback { get; set; }
	}
}