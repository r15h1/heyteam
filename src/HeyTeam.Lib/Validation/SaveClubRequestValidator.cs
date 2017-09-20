using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class SaveClubRequestValidator : IValidator<SaveClubRequest>
    {
        public ValidationResult<SaveClubRequest> Validate(SaveClubRequest request)
        {
            var result = new ValidationResult<SaveClubRequest>(request);
            if(request == null) result.AddMessage("Request cannot be null");
            if(string.IsNullOrWhiteSpace(request.ClubName)) result.AddMessage("Club's name cannot be empty");
            return result;
        }
    }
}