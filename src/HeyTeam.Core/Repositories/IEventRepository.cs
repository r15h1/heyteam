namespace HeyTeam.Core.Repositories {
	public interface IEventRepository {
		void AddEvent(Event @event);
		void UpdateEvent(Event @event);
    }
}
