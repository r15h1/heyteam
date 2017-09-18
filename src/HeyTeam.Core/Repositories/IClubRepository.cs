using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IClubRepository {
        Club Save(Club club);
        Club Get(long value);
    }
}