using HeyTeam.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace HeyTeam.Web.Models.EventsViewModels
{
    public class EventTimeLogViewModel {

		[Required(ErrorMessage = "EventId is required")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "SquadId is required")]
		public Guid SquadId { get; set; }

		[Required(ErrorMessage = "PlayerId is required")]
		public Guid PlayerId { get; set; }

		public short? TimeLogged { get; set; }
	}
}