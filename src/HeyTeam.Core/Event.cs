using System;
using System.Collections.Generic;

namespace HeyTeam.Core {
	public class Event {
		public Event(Guid clubId, Guid? eventId = null) {
			ClubId = clubId;
			Guid = eventId ?? Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid Guid { get; }

		public string Title { get; set; }
		public string Description { get; set; }
		public IEnumerable<Material> Materials { get; set; }
		public string Location { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}