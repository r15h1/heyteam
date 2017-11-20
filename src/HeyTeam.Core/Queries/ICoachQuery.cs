using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface ICoachQuery {
		Coach GetCoach(Guid guid);
		IEnumerable<Coach> GetClubCoaches(Guid clubId);
		IEnumerable<Coach> GetSquadCoaches(Guid squadId);
	}
}
