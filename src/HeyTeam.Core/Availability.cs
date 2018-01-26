using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core
{
    public class Availability {
		public Availability(Guid squadId, Guid playerId) {
			SquadId = squadId;
			PlayerId = playerId;
		}

		public Guid SquadId { get; }
		public string SquadName { get; set; }
		public Guid PlayerId { get; }
		public string PlayerName { get; set; }
		public AvailabilityStatus? AvailabilityStatus { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public string Notes { get; set; }
	}

	public enum AvailabilityStatus {
		Injured = 1,
		OutOfTown = 2,
		Other = 3,
		Available = 4
	}
}