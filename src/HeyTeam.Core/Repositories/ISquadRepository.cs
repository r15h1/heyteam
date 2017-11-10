using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface ISquadRepository {
        void AddSquad(Squad squad);
        void UpdateSquad(Squad squad);
        Squad GetSquad(Guid squadId);
		SquadCoachAssignment GetSquadCoachAssignment(Guid squadId);
		void AssignCoach(Guid squadId, Guid coachId);
		void UnAssignCoach(Guid squadId, Guid coachId);
	}
}