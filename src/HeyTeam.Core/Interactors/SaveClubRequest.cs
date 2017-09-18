using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class SaveClubRequest {
        public long? ClubId { get; set; }
        public string ClubName { get; set; }        
    }

    public class SaveClubResponse {
        public SaveClubResponse(ValidationResult<SaveClubRequest> validationResult, long? clubId = null) {
            ClubId = clubId;
            ValidationResult = validationResult;
        }
        public long? ClubId { get; private set; }        
        public ValidationResult<SaveClubRequest> ValidationResult { get; private set; }
    }
}