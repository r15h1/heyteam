using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IIdentityQuery {
		IEnumerable<User> GetUsers(Guid clubId);
		User GetUserByEmail(Guid clubId, string email);
    }
}
