using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Squad {
    public class AddSquadRequest
    {
        public string SquadName { get; set; }
        public Guid ClubId { get; set; }
    }

    public class AddSquadResponse
    {
        public Guid? SquadId {get;}

        public AddSquadResponse(ValidationResult<AddSquadRequest> result, Guid? squadId = null) {
            this.ValidationResult = result;
            this.SquadId = squadId;
        }

        public ValidationResult<AddSquadRequest> ValidationResult { get; }
    }
}