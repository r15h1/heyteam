using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
	public class TrainingMaterialRequestValidator : IValidator<TrainingMaterialRequest> {
		public ValidationResult<TrainingMaterialRequest> Validate(TrainingMaterialRequest request) {
			var validationResult = new ValidationResult<TrainingMaterialRequest>(request);
			if(request == null) {
				validationResult.AddMessage("Request cannot be null");
				return validationResult;
			}

			if(request.ClubId.IsEmpty())
				validationResult.AddMessage("ClubId cannot be null");

			if(request.Title.IsEmpty())
				validationResult.AddMessage("Title cannot be null");

			if (request.Description.IsEmpty())
				validationResult.AddMessage("Description cannot be null");


			return validationResult;
		}
	}
}
