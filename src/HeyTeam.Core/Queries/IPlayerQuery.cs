using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IPlayerQuery
    {
		Player GetPlayer(Guid playerId);
		IEnumerable<Player> GetPlayers(Guid squadId);
	}
}
