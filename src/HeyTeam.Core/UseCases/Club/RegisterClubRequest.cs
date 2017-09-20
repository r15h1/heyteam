using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class RegisterClubRequest {        
        public string ClubName { get; set; }
        public string ClubLogoUrl { get; set; }
    }

    public class RegisterClubResponse {
        public RegisterClubResponse(ValidationResult<RegisterClubRequest> validationResult, Guid? clubId = null) {
            if (validationResult == null) throw new ArgumentNullException();
            ClubId = clubId;
            ValidationResult = validationResult;
        }
        
        public Guid? ClubId { get; private set; }        
        public ValidationResult<RegisterClubRequest> ValidationResult { get; private set; }
    }
}