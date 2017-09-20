using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IClubRepository {
        Club Save(Club club);
        IList<Club> Get(Guid clubId);
        IList<Club> Get(string nameStartsWith);
    }
}