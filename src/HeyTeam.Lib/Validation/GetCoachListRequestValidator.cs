using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class GetCoachListRequestValidator : IValidator<GetCoachListRequest> {
		public ValidationResult<GetCoachListRequest> Validate(GetCoachListRequest request) {
			var validationResult = new ValidationResult<GetCoachListRequest>(request);

			if (request.ClubId.IsEmpty())
				validationResult.AddMessage("Club Id is not valid");

			return validationResult;
		}
	}
}
