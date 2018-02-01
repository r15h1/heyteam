using System;
using System.Collections.Generic;

namespace HeyTeam.Core {
	public class EventReview {
		public EventReview(Event @event, Guid? eventReviewId = null) {
			
			EventReviewId = eventReviewId.HasValue && eventReviewId.Value != Guid.Empty ? eventReviewId.Value : Guid.NewGuid();
			Event = @event;
		}
		public Event Event { get; }
		public Guid EventReviewId { get; }
		public IList<Guid> Squads { get; set; } = new List<Guid>();
		public Guid CoachId { get; set; }
		public DateTime? LastReviewedOn { get; set; }
		public EventReviewDetails EventReviewDetails { get; set; } = new EventReviewDetails();		
	}

	public class EventReviewDetails {
		public string WhatWentWell { get; set; }
		public string WhatDidNotGoWell { get; set; }
		public string WhatCouldHaveBeenDoneDifferently { get; set; }
	}
}