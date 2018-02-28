using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class TermSetupRequestValidator : IValidator<TermSetupRequest> {
		public ValidationResult<TermSetupRequest> Validate(TermSetupRequest request) {
			var result = new ValidationResult<TermSetupRequest>(request);
			if (request == null)
				result.AddMessage("The term setup request cannot be null");
			
			if(request.Title.IsEmpty())
				result.AddMessage("Title cannot be empty");

			if (request.StartDate >= request.EndDate)
				result.AddMessage("End date must be greater than the start date");

			return result;
		}
	}
}