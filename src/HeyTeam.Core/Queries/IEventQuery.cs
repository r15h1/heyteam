using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEventQuery
    {
		Event GetEvent(Guid eventId);
		IEnumerable<Event> GetEvents(Guid clubId, Guid? squadId = null);
		IEnumerable<EventSummary> GetEventsSummary(Guid clubId);
		IEnumerable<EventSummary> GetEventsSummary(EventsRequest request);
	}

	public class EventsRequest {
		public Guid ClubId { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public int Month { get; set; } = DateTime.Today.Month;
		public int Year { get; set; } = DateTime.Today.Year;
	}
}
