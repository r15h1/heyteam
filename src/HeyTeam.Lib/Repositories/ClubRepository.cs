using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using AutoMapper;
using HeyTeam.Lib.Data;
using Microsoft.EntityFrameworkCore;

namespace HeyTeam.Lib.Repositories {
    public class ClubRepository : IClubRepository
    {
        private readonly IMapper mapper;
        private readonly DbContextOptions<ClubContext> options;

        public ClubRepository(DbContextOptionsBuilder<ClubContext> optionsBuilder) {            
            this.options = optionsBuilder.Options;
            this.mapper = MapperFactory.GetMapper();
        }
        
        public void Add(Core.Entities.Club club)
        {    
            using(var context = new ClubContext(options)) {      
                var clubData = mapper.Map<Data.Club>(club);
                context.Clubs.Add(clubData);  
                context.SaveChanges();          
            }
        }

        public void Update(Core.Entities.Club club)
        {
            using(var context = new ClubContext(options)) {
                var existingClub = context.Clubs.FirstOrDefault(c => c.Guid == club.Guid);
                existingClub.LogoUrl = club.LogoUrl;
                existingClub.Name = club.Name;    
                context.SaveChanges();               
            }
        }

        public Core.Entities.Club Get(Guid clubId)
        {     
            using(var context = new ClubContext(options)) {          
                var club = context.Clubs.Include(c => c.Squads).FirstOrDefault(c => c.Guid == clubId);
                return mapper.Map<Core.Entities.Club>(club);            
            }
        }

        public IList<Core.Entities.Club> Get(string nameStartsWith)
        {
            throw new NotImplementedException();            
        }
    }
}