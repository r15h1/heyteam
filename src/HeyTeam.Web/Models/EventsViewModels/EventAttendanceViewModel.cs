using HeyTeam.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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