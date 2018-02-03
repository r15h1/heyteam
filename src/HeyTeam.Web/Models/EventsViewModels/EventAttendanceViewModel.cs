using HeyTeam.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels
{
    public class EventAttendanceViewModel
    {
		[Required(ErrorMessage = "EventId is required")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "SquadId is required")]
		public Guid SquadId { get; set; }

		[Required(ErrorMessage = "PlayerId is required")]
		public Guid PlayerId { get; set; }

		public Attendance? Attendance { get; set; }
	}
}