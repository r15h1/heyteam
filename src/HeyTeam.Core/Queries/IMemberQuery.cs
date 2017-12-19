using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IMemberQuery
    {
		Player GetPlayer(Guid playerId);
		IEnumerable<Player> GetPlayers(Guid squadId);
		Coach GetCoach(Guid guid);
		IEnumerable<Coach> GetClubCoaches(Guid clubId);
		IEnumerable<Coach> GetSquadCoaches(Guid squadId);
		IEnumerable<Member> GetMembersByEmail(Guid clubId, string email);
	}
}
 