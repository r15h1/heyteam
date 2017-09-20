using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class RegisterClubRequestValidator : IValidator<RegisterClubRequest>
    {
        public ValidationResult<RegisterClubRequest> Validate(RegisterClubRequest request)
        {
            var result = new ValidationResult<RegisterClubRequest>(request);
            if(request == null) result.AddMessage("Request cannot be null");
            if(string.IsNullOrWhiteSpace(request.ClubName)) result.AddMessage("Club name cannot be empty");
            return result;
        }
    }
}