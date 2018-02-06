using System;

namespace HeyTeam.Core.Models {
	public class EventSummary
    {
		public EventSummary(Guid clubId, Guid? eventId = null) {
			ClubId = clubId;
			Guid = eventId ?? Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid Guid { get; }

		public string Title { get; set; }
		public int? TrainingMaterialsCount { get; set; }
		public string Squads { get; set; }
		public string Location { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Attendance? Attendance{ get; set; }
		public EventType EventType { get; set; }
	}
}
