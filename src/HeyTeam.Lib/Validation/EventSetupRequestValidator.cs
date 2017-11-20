using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Validation {
	public class EventSetupRequestValidator : IValidator<EventSetupRequest> {
		public ValidationResult<EventSetupRequest> Validate(EventSetupRequest request) {
			var result = new ValidationResult<EventSetupRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
				return result;
			}

			if (request.ClubId.IsEmpty())
				result.AddMessage("ClubId cannot be empty");

			if (request.Location.IsEmpty())
				result.AddMessage("Street cannot be empty");			

			if (request.StartDate.IsEmpty())
				result.AddMessage("Start Date & Time cannot be empty");
			else if(request.StartDate.Value <= DateTime.Now)
				result.AddMessage("Start Date & Time must be in the future");

			if (request.EndDate.IsEmpty())
				result.AddMessage("End Date & Time cannot be empty");

			if(request.EndDate <= request.StartDate)
				result.AddMessage("End Date must be greater than Start Date");

			if (request.Title.IsEmpty())
				result.AddMessage("Title cannot be empty");

			if (request.Squads == null || request.Squads.Count() == 0)
				result.AddMessage("Squads cannot be empty");
			else if(request.Squads.Any(s => s.IsEmpty()))
				result.AddMessage("One or more of the specified squads has an invalid guid");

			return result;
		}
	}
}
