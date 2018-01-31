using HeyTeam.Core.Models;

namespace HeyTeam.Core.Repositories {
	public interface IAvailabilityRepository {
		void AddAvailability(NewAvailabilityRequest request);
		void DeleteAvailability(DeleteAvailabilityRequest request);
		void UpdateAvailability(UpdateAvailabilityRequest request);
	}
}
