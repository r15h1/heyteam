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
		public IEnumerable<TrainingMaterial> TrainingMaterials { get; set; }
		public IEnumerable<Squad> Squads { get; set; }
		public string Location { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public EventType EventType { get; set; } = EventType.Training;
	}

	public enum EventType {
		Training = 1,
		Exhibition_Game = 2,
		Indoor_League = 3,
		Outdoor_League = 4,
		Tournament = 5,
		Meeting = 6,
		Other = 7
	}

	public static class Extensions {
		private static Dictionary<EventType, string> eventTypeDescriptions = new Dictionary<EventType, string> {
			{ EventType.Exhibition_Game, "Exhibition Game" },
			{ EventType.Indoor_League, "Indoor League" },
			{ EventType.Meeting, "Meeting" },
			{ EventType.Other, "Other" },
			{ EventType.Outdoor_League, "Outdoor League" },
			{ EventType.Tournament, "Tournament" },
			{ EventType.Training, "Training" }
		};

		public static string GetDescription(this EventType eventType) {
			return eventTypeDescriptions.ContainsKey(eventType) ? eventTypeDescriptions[eventType] : string.Empty ;
		}
	}
}