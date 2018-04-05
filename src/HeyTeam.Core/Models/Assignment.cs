using HeyTeam.Core.Models.Mini;
using System;

namespace HeyTeam.Core.Models
{
    public class Assignment
    {
        public Assignment(Guid clubId, Guid assignmentId, Guid playerAssignmentId)
        {
            ClubId = clubId;
            AssignmentId = assignmentId;
            PlayerAssignmentId = playerAssignmentId;
        }

        public Guid ClubId { get; }
        public Guid AssignmentId { get; }
        public Guid PlayerAssignmentId { get; }
        public string Instructions { get; set; }
        public DateTime CreatedOn { get; set; }
        public MiniModel Coach { get; set; }
        public string Title { get; set; }
        public MiniModel Player { get; set; }
        public DateTime? DateDue { get; set; }
    }
}
