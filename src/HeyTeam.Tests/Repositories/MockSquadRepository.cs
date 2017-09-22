using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;

namespace HeyTeam.Tests.Repositories {
    public class MockSquadRepository : ISquadRepository {
        private readonly IEnumerable<Club> clubs;

        public MockSquadRepository(IEnumerable<Club> clubs) {
            this.clubs = clubs;
        }

        public void Add(Squad squad)
        {
            var club = clubs.FirstOrDefault(c => c.Id == squad.Club.Id);
            club.AddSquad(squad);
        }
    }
}