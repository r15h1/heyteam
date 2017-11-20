using System.Collections.Generic;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;
using Dapper;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
    public class ClubRepository : IClubRepository {
        private readonly IDbConnectionFactory connectionFactory;

        public ClubRepository(IDbConnectionFactory factory) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }
        public void AddClub(Core.Entities.Club club) {    
            using (var connection = connectionFactory.Connect()) {
                string sql = @"INSERT INTO CLUBS(Guid, Name, Url) 
                                SELECT @Guid, @Name, @Url 
                                WHERE NOT EXISTS (SELECT 1 FROM CLUBS WHERE Guid = @Guid)"; 

                var p = new DynamicParameters();
                p.Add("@Guid", club.Guid.ToString());
                p.Add("@Name", club.Name);
                p.Add("@Url", club.Url);
                connection.Open();
                connection.Execute(sql, p);
            }
        }

        public void UpdateClub(Core.Entities.Club club) {
            using (var connection = connectionFactory.Connect()) {
                string sql = "UPDATE CLUBS SET Name = @Name, Url = @Url WHERE Guid = @Guid";  
                var p = new DynamicParameters();
                p.Add("@Guid", club.Guid.ToString());
                p.Add("@Name", club.Name);
                p.Add("@Url", club.Url);
                connection.Open();
                connection.Execute(sql, p);
            }
        }
    }
}