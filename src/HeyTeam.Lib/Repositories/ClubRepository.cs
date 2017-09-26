using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;
using Dapper;

namespace HeyTeam.Lib.Repositories {
    public class ClubRepository : IClubRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public ClubRepository(IDbConnectionFactory factory) {
            this.connectionFactory = factory;
        }
        public void Add(Core.Entities.Club club)
        {    
            using (var connection = connectionFactory.Connect())
            {
                string sql = "INSERT INTO CLUBS(Guid, Name, LogoUrl) SELECT @Guid, @Name, @LogoUrl WHERE NOT EXISTS (SELECT 1 FROM CLUBS WHERE Guid = @Guid)";  
                var p = new DynamicParameters();
                p.Add("@Guid", club.Guid);
                p.Add("@Name", club.Name);
                p.Add("@LogoUrl", club.LogoUrl);
                connection.Open();
                connection.Execute(sql, p);
            }
        }

        public void Update(Core.Entities.Club club)
        {
            // using(var context = new ClubContext(options)) {
            //     var existingClub = context.Clubs.FirstOrDefault(c => c.Guid == club.Guid);
            //     existingClub.LogoUrl = club.LogoUrl;
            //     existingClub.Name = club.Name;    
            //     context.SaveChanges();               
            // }
        }

        public Core.Entities.Club Get(Guid clubId)
        {     
            // using(var context = new ClubContext(options)) {          
            //     var club = context.Clubs.Include(c => c.Squads).FirstOrDefault(c => c.Guid == clubId);
            //     return mapper.Map<Core.Entities.Club>(club);            
            // }
            throw new NotImplementedException();
        }

        public IList<Core.Entities.Club> Get(string nameStartsWith)
        {
            throw new NotImplementedException();            
        }
    }
}