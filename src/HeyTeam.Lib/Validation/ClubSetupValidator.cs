using HeyTeam.Core.Interactors;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class ClubSetupValidator : IValidator<ClubSetupRequest>
    {
        public ValidationResult<ClubSetupRequest> Validate(ClubSetupRequest request)
        {
            var result = new ValidationResult<ClubSetupRequest>(request);
            if(request == null) result.AddMessage("Request cannot be null");
            if(string.IsNullOrWhiteSpace(request.ClubName)) result.AddMessage("Club's name cannot be empty");
            return result;
        }
    }
}