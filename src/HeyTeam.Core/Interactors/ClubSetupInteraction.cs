using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubSetupRequest {
        public long? ClubId { get; set; }
        public string ClubName { get; set; }        
    }

    public class ClubSetupResponse {
        public ClubSetupResponse(ValidationResult<ClubSetupRequest> validationResult, long? clubId = null) {
            ClubId = clubId;
            ValidationResult = validationResult;
        }
        public long? ClubId { get; private set; }        
        public ValidationResult<ClubSetupRequest> ValidationResult { get; private set; }
    }
}