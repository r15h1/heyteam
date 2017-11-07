using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Validation {
	public class SaveCoachRequestValidator : IValidator<SaveCoachRequest> {
		public ValidationResult<SaveCoachRequest> Validate(SaveCoachRequest request) {
			var result = new ValidationResult<SaveCoachRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
			} else {
				if(request.Command == SaveCoachRequest.Action.ADD && !request.CoachId.IsEmpty())
					result.AddMessage("CoachId must be empty when creating a new coach");
				else if (request.Command == SaveCoachRequest.Action.UPDATE && request.CoachId.IsEmpty())
					result.AddMessage("CoachId must not be empty");

				if (request.ClubId.IsEmpty()) 
					result.AddMessage("ClubId cannot be null");

				if (request.FirstName.IsEmpty())
					result.AddMessage("First Name cannot be empty");

				if (request.LastName.IsEmpty())
					result.AddMessage("Last Name cannot be empty");

				if (request.Email.IsEmpty() || !request.Email.IsValidEmail())
					result.AddMessage("Email is not valid");

				if (request.Phone.IsEmpty())
					result.AddMessage("Phone is not valid");

				if (request.DateOfBirth >= DateTime.Now.AddYears(-15))
					result.AddMessage("Coach must be at least 15 years old");
			}
			return result;
		}
	}
}
