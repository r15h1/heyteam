using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Squad {
    public class UpdateSquadRequest {
        public Guid ClubId { get; set; }
        public Guid SquadId { get; set; }
        public string SquadName { get; set; }
    }

    public class UpdateSquadResponse {
        public UpdateSquadResponse(ValidationResult<UpdateSquadRequest> validationResult)
        {
            ValidationResult = validationResult;
        }

        public ValidationResult<UpdateSquadRequest> ValidationResult { get; }
    }
}