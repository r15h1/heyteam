using HeyTeam.Core.Models;

namespace HeyTeam.Core.Repositories {
	public interface IAvailabilityRepository {
		void AddAvailability(NewAvailabilityRequest request);
    }
}
