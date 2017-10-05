using System;

namespace HeyTeam.Core.UseCases.Player {
    public class AddPlayerRequest
    {
        public Guid SquadId { get; internal set; }
    }
}