using HeyTeam.Core.Models.Mini;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface ISquadQuery
    {
		IEnumerable<Squad> GetSquads(Guid clubId);
		IEnumerable<SquadSummary> GetSquadSummary(Guid clubId);
		IEnumerable<Squad> GetMemberSquads(Guid memberId, Membership membership);
        Squad GetSquad(Guid squadId);
		(Squad Squad, IEnumerable<Player> Players, Coach Coach) GetFullSquadDetails(Guid squadId);
	}
}