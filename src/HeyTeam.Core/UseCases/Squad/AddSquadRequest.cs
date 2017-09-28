using System;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Squad {
    public class AddSquadRequest
    {
        public string SquadName { get; set; }
        public Guid ClubId { get; set; }
    }
}