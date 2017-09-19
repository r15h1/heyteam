using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Requests {
    public class ClubSaveRequest {
        public long? ClubId { get; set; }
        public string ClubName { get; set; }        
    }

    public class ClubSaveResponse {
        public ClubSaveResponse(ValidationResult<ClubSaveRequest> validationResult, long? clubId = null) {
            ClubId = clubId;
            ValidationResult = validationResult;
        }
        public long? ClubId { get; private set; }        
        public ValidationResult<ClubSaveRequest> ValidationResult { get; private set; }
    }
}