using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class TrainingMaterialReSyncRequestValidator : IValidator<ReSyncRequest> {
		public ValidationResult<ReSyncRequest> Validate(ReSyncRequest request) {
			var validationResult = new ValidationResult<ReSyncRequest>(request);
			if(request == null) {
				validationResult.AddMessage("Request cannot be null");
				return validationResult;
			}

			if(request.ClubId.IsEmpty())
				validationResult.AddMessage("ClubId cannot be null");

			if(request.TrainingMaterialId.IsEmpty())
				validationResult.AddMessage("Training Material Id cannot be null");
				

			return validationResult;
		}
	}
}
