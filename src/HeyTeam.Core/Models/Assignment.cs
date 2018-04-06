using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Models
{
    public class Assignment
    {
        public Assignment(Guid clubId, Guid assignmentId, Guid playerAssignmentId)
        {
            ClubId = clubId;
            AssignmentId = assignmentId;
        }

        public Guid ClubId { get; }
        public Guid AssignmentId { get; }
        public string Instructions { get; set; }        
        public string Title { get; set; }
        public string Createdby { get; set; }
        public string CreatedOn { get; set; }
        public IEnumerable<MiniTrainingMaterial> TrainingMaterials { get; set; } = new List<MiniTrainingMaterial>();
        public IEnumerable<PlayerAssignment> Allocations { get; set; } = new List<PlayerAssignment>();
    }
}
