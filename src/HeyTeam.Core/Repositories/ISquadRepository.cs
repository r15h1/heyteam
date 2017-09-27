using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface ISquadRepository {
        void Add(Squad squad);
        void Update(Squad squad);
    }
}