using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services
{
    public interface IAssignmentService
    {
        Response CreateAssignment(AssignmentRequest request);
        Response RemovePlayerFromAssignment(PlayerAssignmentRequest request);
		Response UpdateAssignment(AssignmentUpdateRequest request);
		Response AddPlayerToAssignment(PlayerAssignmentRequest request);
		Response TrackAssignmentView(AssignmentViewTrackingRequest request);
	}

    public class PlayerAssignmentRequest {
        public Guid ClubId { get; set; }
        public Guid PlayerId { get; set; }
		public Guid AssignmentId { get; set; }
		public Guid? CoachId { get; set; }
	}

    public class AssignmentRequest
    {
		public Guid ClubId { get; set; }
        public Guid? Guid { get; set; }
        public DateTime? SubmittedOn { get; set; }                
        public Guid CoachId { get; set; }
        public DateTime DueDate { get; set; }
        public string Instructions { get; set; }
        public IEnumerable<Guid> TrainingMaterials { get; set; }
        public IEnumerable<Guid> Squads { get; set; }
        public IEnumerable<Guid> Players { get; set; }
        public string Title { get; set; }
		public AssignedTo AssignedTo{ get; set; }
    }	

	public class AssignmentUpdateRequest{
		public Guid ClubId { get; set; }
		public Guid AssignmentId { get; set; }
		public DateTime DueDate { get; set; }
        public string Instructions { get; set; }
        public string Title { get; set; }
        public IEnumerable<Guid> TrainingMaterials { get; set; }
    }

	public class AssignmentViewTrackingRequest{
		public Guid ClubId { get; set; }
		public Guid AssignmentId { get; set; }
		public Guid PlayerId { get; set; }
	}

	public enum AssignedTo {
		All = 0,
		SelectedSquads = 1,
		IndividualPlayers = 2
	}
}