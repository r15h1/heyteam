using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Events {
    public class SquadCoachChanged {
		public SquadCoachChanged(Guid squadId, Guid coachId) {
			SquadId = squadId;
			CoachId = coachId;
		}

		public Guid SquadId { get; }
		public Guid CoachId { get; }
	}
}
