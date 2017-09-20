using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IClubRepository {
        void Add(Club club);
        void Update(Club club);
        Club Get(Guid clubId);
        IList<Club> Get(string nameStartsWith);
    }
}