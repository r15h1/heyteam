using HeyTeam.Core.Models;
using System;

namespace HeyTeam.Core.Services {
	public interface IAvailabilityService {
		Response AddAvailability(NewAvailabilityRequest request);
	}
}