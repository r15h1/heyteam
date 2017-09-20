using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;

namespace HeyTeam.Tests.Repositories {
    public class MockClubRepository : IClubRepository
    {
        private readonly List<Club> clubs;

        public MockClubRepository(){
            clubs = new List<Club>();
        }
        
        public void Add(Club club)
        {            
            clubs.Add(club);
        }

        public void Update(Club club)
        {
            var existingClub = clubs.FirstOrDefault(c => c.Id == club.Id);
            existingClub.LogoUrl = club.LogoUrl;
            existingClub.Name = club.Name;
        }

        public Club Get(Guid clubId)
        {
            return clubs.FirstOrDefault(c => c.Id == clubId);
        }

        public IList<Club> Get(string nameStartsWith)
        {
            return string.IsNullOrWhiteSpace(nameStartsWith) ? clubs : clubs.Where(c => c.Name.StartsWith(nameStartsWith)).ToList();
        }
    }
}