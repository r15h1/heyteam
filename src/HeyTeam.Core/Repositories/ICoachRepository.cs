using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Repositories {
	public interface ICoachRepository {
		Coach GetCoach(Guid guid);
		void AddCoach(Coach coach);
		IEnumerable<Coach> GetCoaches(Guid clubId);
		void UpdateCoach(Coach coach);
	}
}
