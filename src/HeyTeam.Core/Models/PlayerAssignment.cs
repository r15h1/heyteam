using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Models
{
    public class PlayerAssignment
    {
        public PlayerAssignment(Guid playerId, Guid assignmentId, Guid playerAssignmentId)
        {
            PlayerId = playerId;
            AssignmentId = assignmentId;
            PlayerAssignmentId = playerAssignmentId;
        }
        public string PlayerName { get; set; }
        public string AssignedOn { get; set; }
        public string  AssignedBy { get; set; }
        public Guid PlayerId { get; }
        public Guid AssignmentId { get; }
        public Guid PlayerAssignmentId { get; }
        public string DueDate { get; set; }
    }
}
