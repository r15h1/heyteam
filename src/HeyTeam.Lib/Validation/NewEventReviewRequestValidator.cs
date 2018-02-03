using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation
{
    public class NewEventReviewRequestValidator : IValidator<NewEventReviewRequest> {
		public ValidationResult<NewEventReviewRequest> Validate(NewEventReviewRequest request) {
            var validationResult = new ValidationResult<NewEventReviewRequest>(request);
            
            if(request == null) {
                validationResult.AddMessage("Request cannot be null");
                return validationResult;
            }

            if (request.ClubId.IsEmpty())
                validationResult.AddMessage("ClubId cannot be empty");

            if(request.CoachId.IsEmpty())
                validationResult.AddMessage("CoachId cannot be empty");

            if (request.EventId.IsEmpty())
                validationResult.AddMessage("EventId cannot be empty");

            if (request.DifferentNextTime.IsEmpty() && request.Opportunities.IsEmpty() && request.Successes.IsEmpty())
                validationResult.AddMessage("No review meesage has been provided");

            return validationResult;
		}
	}
}
