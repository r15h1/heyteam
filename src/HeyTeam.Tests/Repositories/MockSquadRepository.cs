using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;

namespace HeyTeam.Tests.Repositories {
    public class MockSquadRepository : ISquadRepository {
        private readonly IEnumerable<Club> clubs;

        public MockSquadRepository(IEnumerable<Club> clubs) {
            this.clubs = clubs;
        }
    }
}