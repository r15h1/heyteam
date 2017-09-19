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
            if(!club.Id.HasValue) 
                return AddNew(club);
            
            return Update(club);
        }

        private Club Update(Club club)
        {
            throw new NotImplementedException();
        }

        private Club AddNew(Club club)
        {
            var newId = (clubs.Max(c => c.Id) ?? 0) + 1;
            var newClub = new Club(newId) { Name = club.Name };
            clubs.Add(newClub);
            return newClub;
        }

        IList<Club> IClubRepository.Get(long clubId)
        {
            return clubs.Where(c => c.Id == clubId).ToList();
        }

        public IList<Club> Get(string nameStartsWith)
        {
            return string.IsNullOrWhiteSpace(nameStartsWith) ? clubs : clubs.Where(c => c.Name.StartsWith(nameStartsWith)).ToList();
        }
    }
}