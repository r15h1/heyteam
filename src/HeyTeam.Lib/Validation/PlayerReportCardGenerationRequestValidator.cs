using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class PlayerReportCardGenerationRequestValidator : IValidator<PlayerReportCardGenerationRequest> {
		public ValidationResult<PlayerReportCardGenerationRequest> Validate(PlayerReportCardGenerationRequest request) {
			var validationResult = new ValidationResult<PlayerReportCardGenerationRequest>(request);
			if(request == null){
				validationResult.AddMessage("Request cannot be null");
				return validationResult;
			}

			if (request.ClubId.IsEmpty())
				validationResult.AddMessage("ClubId cannot be empty");

			if (request.PlayerId.IsEmpty())
				validationResult.AddMessage("PlayerId cannot be empty");

			if (request.ReportDesignId.IsEmpty())
				validationResult.AddMessage("ReportDesignId cannot be empty");

			if (request.SquadId.IsEmpty())
				validationResult.AddMessage("SquadId cannot be empty");

			if (request.TermId.IsEmpty())
				validationResult.AddMessage("TermId cannot be empty");

			return validationResult;
		}
	}
}
