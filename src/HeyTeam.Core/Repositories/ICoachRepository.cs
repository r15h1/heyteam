using HeyTeam.Core.Entities;
using System;

namespace HeyTeam.Core.Repositories {
	public interface ICoachRepository {
		Coach GetCoach(Guid guid);
		void AddCoach(Coach coach);
	}
}
