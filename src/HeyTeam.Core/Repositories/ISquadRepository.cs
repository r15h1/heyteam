using System;

namespace HeyTeam.Core.Repositories {
	public interface ISquadRepository {
        void AddSquad(Squad squad);
        void UpdateSquad(Squad squad);        
		void AssignCoach(Guid squadId, Guid coachId);
		void UnAssignCoach(Guid squadId, Guid coachId);		
	}
}