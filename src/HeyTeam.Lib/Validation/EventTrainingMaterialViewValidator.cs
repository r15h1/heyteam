using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
	public class EventTrainingMaterialViewValidator : IValidator<EventTrainingMaterialViewRequest>
    {
        public ValidationResult<EventTrainingMaterialViewRequest> Validate(EventTrainingMaterialViewRequest request)
        {
            return new ValidationResult<EventTrainingMaterialViewRequest>(request);
        }
    }
}
