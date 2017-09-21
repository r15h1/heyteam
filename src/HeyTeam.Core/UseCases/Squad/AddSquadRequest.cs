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
        public AddSquadResponse(ValidationResult<AddSquadRequest> result) {
            this.ValidationResult = result;
        }

        public ValidationResult<AddSquadRequest> ValidationResult { get; }
    }
}