using HeyTeam.Core.Interactors;
using HeyTeam.Core.Requests;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class ClubSaveRequestValidator : IValidator<ClubSaveRequest>
    {
        public ValidationResult<ClubSaveRequest> Validate(ClubSaveRequest request)
        {
            var result = new ValidationResult<ClubSaveRequest>(request);
            if(request == null) result.AddMessage("Request cannot be null");
            if(string.IsNullOrWhiteSpace(request.ClubName)) result.AddMessage("Club's name cannot be empty");
            return result;
        }
    }
}