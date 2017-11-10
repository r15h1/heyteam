using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
	public class SquadCoachChangeRequestValidator : IValidator<SquadCoachChangeRequest> {
		public ValidationResult<SquadCoachChangeRequest> Validate(SquadCoachChangeRequest request) {
			var validationResult = new ValidationResult<SquadCoachChangeRequest>(request);
			if (request == null) validationResult.AddMessage("Request cannot be null");
			if (request.SquadId.IsEmpty()) validationResult.AddMessage("Squad Id must be provided");
			if (request.CoachId.IsEmpty()) validationResult.AddMessage("Coach Id must be provided");

			return validationResult;
		}
	}
}
