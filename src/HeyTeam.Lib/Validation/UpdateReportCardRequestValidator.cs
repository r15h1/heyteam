using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
    public class UpdateReportCardRequestValidator : IValidator<UpdateReportCardRequest>
    {
        public ValidationResult<UpdateReportCardRequest> Validate(UpdateReportCardRequest request)
        {
            var validationResult = new ValidationResult<UpdateReportCardRequest>(request);
            if (request == null)
            {
                validationResult.AddMessage("Request cannot be null");
                return validationResult;
            }

            if (request.ClubId.IsEmpty())
                validationResult.AddMessage("ClubId cannot be empty");

            if (request.PlayerId.IsEmpty())
                validationResult.AddMessage("PlayerId cannot be empty");

            if (request.ReportCardId.IsEmpty())
                validationResult.AddMessage("ReportCardId cannot be empty");

            if (request.SquadId.IsEmpty())
                validationResult.AddMessage("SquadId cannot be empty");

            if (request.TermId.IsEmpty())
                validationResult.AddMessage("TermId cannot be empty");            

            return validationResult;
        }
    }
}
