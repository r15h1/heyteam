using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class EventTimeLogRequestValidator : IValidator<EventTimeLogRequest> {
		public ValidationResult<EventTimeLogRequest> Validate(EventTimeLogRequest request) {
			var result = new ValidationResult<EventTimeLogRequest>(request);

			if (request.SquadId.IsEmpty())
				result.AddMessage("Squad Id cannot be empty");

			if (request.ClubId.IsEmpty())
				result.AddMessage("Club Id cannot be empty");

			if (request.EventId.IsEmpty())
				result.AddMessage("Event Id cannot be empty");

			if (request.PlayerId.IsEmpty())
				result.AddMessage("Player Id cannot be empty");

			return result;
		}
	}
}

