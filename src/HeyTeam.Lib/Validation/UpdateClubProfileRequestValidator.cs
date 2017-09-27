using System;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Lib.Validation {
    public class UpdateClubProfileRequestValidator : IValidator<UpdateClubProfileRequest>
    {        
        public ValidationResult<UpdateClubProfileRequest> Validate(UpdateClubProfileRequest request)
        {
            var validationResult = new ValidationResult<UpdateClubProfileRequest>(request);            
            if (request == null) validationResult.AddMessage("Request cannot be null");
            if (request.ClubId.IsEmpty()) validationResult.AddMessage("Club Id must be provided");
            if (string.IsNullOrWhiteSpace(request.ClubName)) validationResult.AddMessage("Club name cannot be empty");
            if(!string.IsNullOrWhiteSpace(request.ClubLogoUrl) && !request.ClubLogoUrl.IsValidUrl()) validationResult.AddMessage("The logo url is not valid");
            return validationResult;
        }
    }
}