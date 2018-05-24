using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEventQuery
    {
		Event GetEvent(Guid eventId);
		IEnumerable<Event> GetEvents(Guid clubId, Guid? squadId = null);
		IEnumerable<EventSummary> GetUpcomingEvents(UpcomingEventsRequest request);
		IEnumerable<EventSummary> GetEventsSummary(Guid clubId);
		IEnumerable<EventSummary> GetEventsSummary(EventsRequest request);
		IEnumerable<EventPlayer> GetPlayersByEvent(Guid eventId);
		IEnumerable<EventReview> GetEventReviews(Guid eventId);
		IEnumerable<Squad> GetUnReviewedSquads(Guid eventId);
		EventReport GetEventReport(Guid eventId);
		IEnumerable<TrainingMaterialView> GetTrainingMaterialViews(Guid eventId);
	}

	public class EventsRequest {
		public Guid ClubId { get; set; }
		public Guid? SquadId { get; set; }
		public Guid? PlayerId { get; set; }
		public int Month { get; set; } = DateTime.Today.Month;
		public int Year { get; set; } = DateTime.Today.Year;
	}

	public class UpcomingEventsRequest {
		public Guid ClubId { get; set; }		
		public Guid MemberId{ get; set; }
		public Membership Membership{ get; set; }
		public int Limit { get; set; } = 5;
	}
}
