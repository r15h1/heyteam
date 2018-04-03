﻿using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services
{
    public interface IAssignmentService
    {
        Response CreateAssignment(AssignmentRequest request);
    }

    public class AssignmentRequest
    {
        public Guid? Guid { get; set; }
        public DateTime? SubmittedOn { get; set; }                
        public Guid CoachId { get; set; }
        public DateTime? DateDue { get; set; }
        public string Notes { get; set; }
        public IEnumerable<Guid> TrainingMaterials { get; set; }
        public IEnumerable<Guid> Squads { get; set; }
        public IEnumerable<Guid> Players { get; set; }
    }
}