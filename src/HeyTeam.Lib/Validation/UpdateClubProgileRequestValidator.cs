using System;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;

namespace HeyTeam.Lib.Validation {
    public class UpdateClubProfileRequestValidator : IValidator<UpdateClubProfileRequest>
    {        
        public ValidationResult<UpdateClubProfileRequest> Validate(UpdateClubProfileRequest request)
        {
            var validationResult = new ValidationResult<UpdateClubProfileRequest>(request);            
            if (request == null) validationResult.AddMessage("Request cannot be null");
            if (request.ClubId == null) validationResult.AddMessage("Club Id must be provided");
            if (string.IsNullOrWhiteSpace(request.ClubName)) validationResult.AddMessage("Club name cannot be empty");
            return validationResult;
        }
    }
}