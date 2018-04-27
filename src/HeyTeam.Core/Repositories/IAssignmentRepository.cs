using System;
using HeyTeam.Core.Services;

namespace HeyTeam.Core.Repositories {
	public interface IAssignmentRepository
    {
		void CreateAssignment(AssignmentRequest request);
        void DeletePlayerAssignment(UnAssignPlayerRequest request);
		void UpdateAssignment(AssignmentUpdateRequest request);
	}
}
