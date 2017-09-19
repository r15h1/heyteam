using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Requests {
    public class ClubSquadRequest {
        public long? ClubId { get; set; }
        public long? SquadId { get; set; }
        public string SquadName {get; set; }
    }

    public class ClubSquadResponse {
        public ClubSquadResponse(ValidationResult<ClubSquadRequest> validationResult, long? squadId = null) {
            SquadId = squadId;
            ValidationResult = validationResult;
        }
        public long? SquadId { get; private set; }        
        public ValidationResult<ClubSquadRequest> ValidationResult { get; private set; }
    }
}