using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Repositories {
	public interface ICoachRepository {
		Coach GetCoach(Guid guid);
		void AddCoach(Coach coach);
		IEnumerable<Coach> GetClubCoaches(Guid clubId);
		IEnumerable<Coach> GetSquadCoaches(Guid squadId);
		void UpdateCoach(Coach coach);
	}
}
