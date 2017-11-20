using HeyTeam.Core.Entities;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface ISquadQuery
    {
		IEnumerable<Squad> GetSquads(Guid clubId);
		Squad GetSquad(Guid squadId);
		(Squad Squad, IEnumerable<Player> Players, Coach Coach) GetFullSquadDetails(Guid squadId);
	}
}
