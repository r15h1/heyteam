using System;
using System.Collections.Generic;

namespace HeyTeam.Core {
	public class EventReview {

		public EventReview(Guid eventId, Guid? eventReviewId = null) {
			EventId = eventId;
			EventReviewId = eventReviewId ?? Guid.NewGuid();
		}

		public Guid EventId { get; }
		public Guid EventReviewId { get; }
		public ICollection<Squad> Squads { get; set; } = new List<Squad>();
		public Coach Coach { get; set; }
		public DateTime? LastReviewedOn { get; set; }		
		public string Successes { get; set; }//What went well
		public string Opportunities { get; set; }//What did not go well
		public string DifferentNextTime { get; set; }//What could have been done differently
	}
}