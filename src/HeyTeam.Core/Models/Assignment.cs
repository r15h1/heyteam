using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Models
{
    public class Assignment
    {
        public Assignment(Guid clubId, Guid assignmentId)
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
        public string DueDate { get; set; }
        public int Players { get; set; }
        public int TrainingMaterials { get; set; }
    }
}
