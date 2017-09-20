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
        
        public Club Save(Club club)
        {            
            if(clubs.Any(c => c.Id == club.Id))
                return Update(club);            
            
            clubs.Add(club);
            return club;
        }

        private Club Update(Club club)
        {
            var existingClub = clubs.FirstOrDefault(c => c.Id == club.Id);
            existingClub.LogoUrl = club.LogoUrl;
            existingClub.Name = club.Name;
            return existingClub;
        }

        IList<Club> IClubRepository.Get(Guid clubId)
        {
            return clubs.Where(c => c.Id == clubId).ToList();
        }

        public IList<Club> Get(string nameStartsWith)
        {
            return string.IsNullOrWhiteSpace(nameStartsWith) ? clubs : clubs.Where(c => c.Name.StartsWith(nameStartsWith)).ToList();
        }
    }
}