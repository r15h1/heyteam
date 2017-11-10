using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.UseCases.Squad
{
    public class SquadCoachChangeRequest {
		public Guid SquadId { get; set; }
		public Guid CoachId { get; set; }
		public SquadCoachOperation Operation { get; set; }

		public enum SquadCoachOperation {
			ADD,
			REMOVE
		}
	}
}
