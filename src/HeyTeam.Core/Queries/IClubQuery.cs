using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IClubQuery
    {
		Club GetClub(Guid clubId);
		bool IsUrlAlreadyAssigned(string url, Guid? clubId = null);
		IEnumerable<Club> GetClubs();
		IEnumerable<Member> GetMembersByEmail(Guid clubId, string email);
	}
}
