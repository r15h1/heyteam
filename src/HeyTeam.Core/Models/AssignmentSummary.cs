using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Models
{
    public class AssignmentSummary
    {
        public AssignmentSummary(Guid clubId, Guid assignmentId)
        {
            ClubId = clubId;
            AssignmentId = assignmentId;
        }

        public Guid ClubId { get; }
        public Guid AssignmentId { get; }
        public string Instructions { get; set; }        
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FormattedDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public int PlayerCount { get; set; }
        public int TrainingMaterialCount { get; set; }
		public string Squads { get; set; }
	}
}
