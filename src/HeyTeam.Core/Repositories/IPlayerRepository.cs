using System;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IPlayerRepository {
        void Add(Player player);
        Player Get(Guid guid);
    }
}