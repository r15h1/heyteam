using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Validation {
	public class AssignmentRequestValidator : IValidator<AssignmentRequest> {
		public ValidationResult<AssignmentRequest> Validate(AssignmentRequest request) {
			var validationResult = new ValidationResult<AssignmentRequest>(request);
			if(request == null){
				validationResult.AddMessage("Request cannot be null");
				return validationResult;
			}

			if(request.Notes.IsEmpty()){
				validationResult.AddMessage("Notes cannot be empty");
			}

			if ((request.Squads == null || request.Squads.Count() == 0) && (request.Players == null || request.Players.Count() == 0)) {
				validationResult.AddMessage("Assignment must be allocated to squad(s) and/or player(s)");
			}

			if (request.DateDue.HasValue && request.DateDue.Value < DateTime.Today) {
				validationResult.AddMessage("Date due must be in the future");
			}

			return validationResult;
		}
	}
}
