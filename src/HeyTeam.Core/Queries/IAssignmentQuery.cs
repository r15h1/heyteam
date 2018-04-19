using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries
{
    public interface IAssignmentQuery
    {
        IEnumerable<AssignmentSummary> GetAssignments(AssignmentsRequest request);
        AssignmentSummary GetPlayerAssignment(PlayerAssignmentRequest request);
		AssignmentDetails GetAssignment(Guid clubId, Guid assignmentId);
	}

	public class AssignmentsRequest {
		public Guid ClubId { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public IEnumerable<Guid> Players { get; set; }
		public int? Month { get; set; }//= DateTime.Today.Month;
		public int? Year { get; set; } //= DateTime.Today.Year;
	}

	public class PlayerAssignmentRequest{
		public Guid ClubId { get; set; }
		public Guid AssignmentId { get; set; }
        public Guid PlayerId { get; set; }
    }
}