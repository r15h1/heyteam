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
                p.Add("@Guid", club.Guid.ToString());
                p.Add("@Name", club.Name);
                p.Add("@LogoUrl", club.LogoUrl);
                connection.Open();
                connection.Execute(sql, p);
            }
        }

        public void Update(Core.Entities.Club club)
        {
            using (var connection = connectionFactory.Connect())
            {
                string sql = "UPDATE CLUBS SET Name = @Name, LogoUrl = @LogoUrl WHERE Guid = @Guid";  
                var p = new DynamicParameters();
                p.Add("@Guid", club.Guid.ToString());
                p.Add("@Name", club.Name);
                p.Add("@LogoUrl", club.LogoUrl);
                connection.Open();
                connection.Execute(sql, p);
            }
        }

        public Club Get(Guid clubId)
        {     
            using (var connection = connectionFactory.Connect())
            {
                string sql = "SELECT Guid, Name, LogoUrl FROM Clubs WHERE Guid = @Guid";  
                var p = new DynamicParameters();
                p.Add("@Guid", clubId.ToString());
                connection.Open();
                var results = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                return results.Select(row => 
                        new Club(Guid.Parse(row["Guid"].ToString())){ 
                            Name = (string)row["Name"], 
                            LogoUrl = (string)row["LogoUrl"]}
                        ).FirstOrDefault();
            }
        }

        public IList<Core.Entities.Club> Get(string nameStartsWith)
        {
            throw new NotImplementedException();            
        }
    }
}