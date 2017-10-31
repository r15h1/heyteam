using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Squad {
    public class GetSquadRequest {
        public Guid ClubId { get; set; }
        public Guid SquadId { get; set; }
    }
}