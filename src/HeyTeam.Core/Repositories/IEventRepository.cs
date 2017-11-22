using System;

namespace HeyTeam.Core.Repositories {
	public interface IEventRepository {
		void AddEvent(Event @event);
		void UpdateEvent(Event @event);
		void DeleteEvent(Guid clubId, Guid eventId);
	}
}
