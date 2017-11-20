using HeyTeam.Core.UseCases.Events;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
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

			if (request.Street.IsEmpty())
				result.AddMessage("Street cannot be empty");

			if (request.City.IsEmpty())
				result.AddMessage("City cannot be empty");			

			if (request.Province.IsEmpty())
				result.AddMessage("Province cannot be empty");

			if (request.PostalCode.IsEmpty())
				result.AddMessage("PostalCode cannot be empty");

			if (request.Country.IsEmpty())
				result.AddMessage("Country cannot be empty");

			if (request.StartDate.IsEmpty())
				result.AddMessage("Start Date & Time cannot be empty");

			if (request.EndTime.IsEmpty())
				result.AddMessage("End Date & Time cannot be empty");

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
