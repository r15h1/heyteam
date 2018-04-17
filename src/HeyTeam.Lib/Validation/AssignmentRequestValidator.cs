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
            if (request.Title.IsEmpty()) validationResult.AddMessage("Title cannot be empty");

            if (request.Instructions.IsEmpty()) validationResult.AddMessage("Notes cannot be empty");

            if (request.AssignedTo == AssignedTo.SelectedSquads && (request.Squads == null || request.Squads.Count() == 0))
                validationResult.AddMessage("Assignment must be allocated to at least one squad");
			else if (request.AssignedTo == AssignedTo.IndividualPlayers && (request.Players == null || request.Players.Count() == 0))
				validationResult.AddMessage("Assignment must be allocated to at least one player");

			if (request.DueDate < DateTime.Today)
                validationResult.AddMessage("Date due must be in the future");

            if (request.CoachId.IsEmpty())
                validationResult.AddMessage("CoachId must be supplied");

            return validationResult;
		}
	}
}
