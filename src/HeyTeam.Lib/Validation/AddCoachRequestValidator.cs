using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Validation {
	public class AddCoachRequestValidator : IValidator<AddCoachRequest> {
		public ValidationResult<AddCoachRequest> Validate(AddCoachRequest request) {
			var result = new ValidationResult<AddCoachRequest>(request);
			if (request == null) {
				result.AddMessage("Request cannot be null");
			} else {
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
