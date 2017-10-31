using System;
using HeyTeam.Core.UseCases.Club;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using static HeyTeam.Core.UseCases.Club.UpdateClubProfileRequest;

namespace HeyTeam.Lib.Validation {
    public class UpdateClubProfileRequestValidator : IValidator<UpdateClubProfileRequest>
    {        
        public ValidationResult<UpdateClubProfileRequest> Validate(UpdateClubProfileRequest request) {
            var validationResult = new ValidationResult<UpdateClubProfileRequest>(request);            
            if (request == null) validationResult.AddMessage("Request cannot be null");
            if (request.ClubId.IsEmpty()) validationResult.AddMessage("Club Id must be provided");
            
            if(request.Fields.Count == 0)
                validationResult.AddMessage("There is nothing to update");

            if (request.Fields.ContainsKey(UpdatableFields.NAME))
                if(request.Fields[UpdatableFields.NAME].IsEmpty()) 
                    validationResult.AddMessage("Club name cannot be empty");

            if (request.Fields.ContainsKey(UpdatableFields.URL))
                if(request.Fields[UpdatableFields.URL].IsEmpty() || !request.Fields[UpdatableFields.URL].IsValidUrl()) 
                    validationResult.AddMessage("The url is not valid");
                    
            return validationResult;
        }
    }
}