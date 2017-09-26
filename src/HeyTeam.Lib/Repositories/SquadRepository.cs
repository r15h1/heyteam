using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using Microsoft.EntityFrameworkCore;
using HeyTeam.Lib.Data;
using AutoMapper;

namespace HeyTeam.Lib.Repositories {
    public class SquadRepository : ISquadRepository
    {
        private readonly DbContextOptions<ClubContext> options;
        private readonly IMapper mapper;

        public SquadRepository(DbContextOptionsBuilder<ClubContext> optionsBuilder) {            
            this.options = optionsBuilder.Options;
            this.mapper = MapperFactory.GetMapper();
        }

        public void Add(Core.Entities.Squad squad)
        {
            using(var context = new ClubContext(options)) {      
                //var squadData = mapper.Map<Data.Squad>(squad);
                var squadData = new Squad { ClubId = squad.Club.ClubId,Guid = squad.Guid, Name = squad.Name };
                context.Squads.Add(squadData);  
                context.SaveChanges();          
            }
        }
    }
}