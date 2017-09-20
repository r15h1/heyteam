using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class SaveClubRequest {
        public Guid? ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogoUrl { get; set; }
    }

    public class SaveClubResponse {
        public SaveClubResponse(ValidationResult<SaveClubRequest> validationResult, Guid? clubId = null) {
            ClubId = clubId;
            ValidationResult = validationResult;
        }
        public Guid? ClubId { get; private set; }        
        public ValidationResult<SaveClubRequest> ValidationResult { get; private set; }
    }
}