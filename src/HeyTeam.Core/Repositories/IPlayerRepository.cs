using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IPlayerRepository {
        void AddPlayer(Player player);
        Player GetPlayer(Guid playerId);
        IEnumerable<Player> GetPlayers(Guid squadId);
    }
}