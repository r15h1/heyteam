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
            
            Guid barcaId = Guid.NewGuid();
            Guid manUtdId = Guid.NewGuid();
            
            ClubRepository.Add(GetBarcelona(barcaId));
            ClubRepository.Add(GetManutd(manUtdId));

            Clubs = new List<Club>{ GetBarcelona(barcaId), GetManutd(manUtdId) };
            SquadRepository = new MockSquadRepository(new List<Club>{ GetBarcelona(barcaId), GetManutd(manUtdId) });
        }

        private Club GetBarcelona(Guid id) => new Club(id) { Name = "Barcelona", LogoUrl = "http://www.logo.com/barca.jpeg"};        
        private Club GetManutd(Guid id) => new Club(id) { Name = "Manchester United", LogoUrl = "http://www.logo.com/manutd.jpeg"};

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