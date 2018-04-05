using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries
{
    public interface IAssignmentQuery
    {
        IEnumerable<Assignment> GetAssignments(AssignmentsRequest request);
    }

	public class AssignmentsRequest {
		public Guid ClubId { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public int Month { get; set; } = DateTime.Today.Month;
		public int Year { get; set; } = DateTime.Today.Year;
	}
}