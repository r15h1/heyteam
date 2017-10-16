using HeyTeam.Core.UseCases;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
    public class DashboardRequestValidator : IValidator<DashboardRequest>
    {
        public ValidationResult<DashboardRequest> Validate(DashboardRequest request)
        {
            var result = new ValidationResult<DashboardRequest>(request);
            if (request.UserEmail.IsEmpty() || !request.UserEmail.IsValidEmail())
                result.AddMessage("User's email is not valid");

            if (request.ClubId.IsEmpty())
                result.AddMessage("Club Id is not valid");

            return result;
        }
    }
}