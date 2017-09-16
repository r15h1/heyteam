using HeyTeam.Core.Entities;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class SquadCreationRequest {
        public long ClubId { get; set; }
        public string SquadName { get; set; }        
    }

    public class SquadCreationResponse {       
        public SquadCreationResponse (ValidationResult<Squad> validationResult, long? squadId) {
            ValidationResult = validationResult;
            SquadId = squadId;
        }

        public ValidationResult<Squad> ValidationResult { get; private set; }
        public long? SquadId { get; private set; }
    }
}