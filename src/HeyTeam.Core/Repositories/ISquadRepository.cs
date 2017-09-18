using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface ISquadRepository
    {
        Squad Save(Squad squad);
    }
}