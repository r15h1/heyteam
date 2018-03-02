using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
    public class NewReportDesignRequestValidator : IValidator<NewReportDesignRequest>
    {
        public ValidationResult<NewReportDesignRequest> Validate(NewReportDesignRequest request)
        {
            var validationResult = new ValidationResult<NewReportDesignRequest>(request);

            if (request == null) {
                validationResult.AddMessage("Request cannot be null");
                return validationResult;
            }

            if(request.ClubId.IsEmpty())
                validationResult.AddMessage("ClubId cannot be empty");

            if (request.Name.IsEmpty())
                validationResult.AddMessage("Name cannot be empty");

            return validationResult;
        }
    }
}
