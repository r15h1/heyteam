using System;
using HeyTeam.Core.Services;

namespace HeyTeam.Core.Repositories {
	public interface IAssignmentRepository
    {
		void CreateAssignment(AssignmentRequest request);
        void DeletePlayerAssignment(PlayerAssignmentRequest request);
		void UpdateAssignment(AssignmentUpdateRequest request);
		void AddPlayerToAssignment(PlayerAssignmentRequest request);
	}
}
