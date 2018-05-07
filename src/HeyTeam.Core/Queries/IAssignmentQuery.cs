using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries
{
    public interface IAssignmentQuery
    {
        IEnumerable<AssignmentSummary> GetAssignments(AssignmentsRequest request);
        AssignmentSummary GetPlayerAssignment(PlayerAssignmentQuery request);
		AssignmentDetails GetAssignment(Guid clubId, Guid assignmentId);
	}

	public class AssignmentsRequest {
		public Guid ClubId { get; set; }
		public Guid? SquadId { get; set; }
		public Guid? PlayerId { get; set; }
		public DateTime? Date{ get; set; }
	}

	public class PlayerAssignmentQuery{
		public Guid ClubId { get; set; }
		public Guid AssignmentId { get; set; }
        public Guid PlayerId { get; set; }
    }
}