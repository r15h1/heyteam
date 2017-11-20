using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEventQuery
    {
		Event GetEvent(Guid eventId);
		IEnumerable<Event> GetEvents(Guid clubId, Guid? squadId = null);
	}
}
