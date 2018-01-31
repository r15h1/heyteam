using HeyTeam.Core.Models;

namespace HeyTeam.Core.Services
{
    public interface IAvailabilityService {
		Response AddAvailability(NewAvailabilityRequest request);
        Response DeleteAvailability(DeleteAvailabilityRequest request);
    }
}