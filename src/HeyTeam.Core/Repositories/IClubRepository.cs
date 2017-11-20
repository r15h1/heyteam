using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;

namespace HeyTeam.Core.Repositories {
    public interface IClubRepository {
        void AddClub(Club club);
        void UpdateClub(Club club);
    }
}