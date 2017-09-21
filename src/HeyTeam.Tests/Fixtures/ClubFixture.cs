using System;
using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Tests.Repositories;
using Xunit;

namespace HeyTeam.Tests.Fixtures {
    public class ClubFixture : IDisposable
    {
        public ClubFixture() {
            ClubRepository = new MockClubRepository();
            Club club1 = new Club { Name = "Barcelona", LogoUrl = "http://www.logo.com/barca.jpeg"};
            Club club2 = new Club { Name = "Manchester United", LogoUrl = "http://www.logo.com/manutd.jpeg"};
            ClubRepository.Add(club1);
            ClubRepository.Add(club2);
            Clubs = new List<Club>{ club1, club2 };
            SquadRepository = new MockSquadRepository(new List<Club>{ club1, club2 });
        }

        public IClubRepository ClubRepository { get; private set; }
        public ISquadRepository SquadRepository {get; private set; }
        public List<Club> Clubs {get; private set; }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    [CollectionDefinition("ClubFixtureCollection")]
    public class ClubFixtureCollection : ICollectionFixture<ClubFixture> { }
}