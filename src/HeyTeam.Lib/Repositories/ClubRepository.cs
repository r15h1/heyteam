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
            Ensure.ArgumentNotNull(factory);
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

        public Club GetClub(Guid clubId) {     
            using (var connection = connectionFactory.Connect()) {
                string sql = @"SELECT Guid, Name, Url FROM Clubs WHERE Guid = @Guid; 
                                    SELECT S.Guid, S.Name FROM Squads S 
                                        INNER JOIN Clubs C ON S.ClubId = C.ClubId 
                                        WHERE C.Guid = @Guid;";  

                var p = new DynamicParameters();
                p.Add("@Guid", clubId.ToString());
                connection.Open();

                using (var multi = connection.QueryMultiple(sql, p)) {
                    var club = multi.Read().Cast<IDictionary<string, object>>().Select(row => 
                        new Club(Guid.Parse(row["Guid"].ToString())){ 
                            Name = (string)row["Name"], 
                            Url = (string)row["Url"]}
                        ).FirstOrDefault();
                    
                    var squads = multi.Read().Cast<IDictionary<string, object>>().Select(row => 
                        new Squad(club.Guid, Guid.Parse(row["Guid"].ToString())) {
                        Name = (string)row["Name"]
                    }).ToList();

                    if(squads != null && squads.Count > 0)
                        squads.ForEach((s) => club.AddSquad(s));

                    return club;
                }
            }
        }

        public bool IsUrlAlreadyAssigned(string url, Guid? clubId = null) {
            using (var connection = connectionFactory.Connect()) {
                string sql = "SELECT COUNT(1) FROM CLUBS WHERE Url = @Url" + (clubId.IsEmpty() ? "" : $" AND Guid != @Guid" );  
                var p = new DynamicParameters();
                p.Add("@Url", url);
                
                if(!clubId.IsEmpty())
                    p.Add("@Guid", clubId.Value.ToString());

                connection.Open();
                long count = (long)connection.ExecuteScalar(sql, p);
                return count > 0;
            }
        }

        public IEnumerable<Club> GetClubs()
        {
            using (var connection = connectionFactory.Connect()) {
                string sql = @"SELECT Guid, Name, Url FROM Clubs";  
                connection.Open();
                var reader = connection.Query(sql).Cast<IDictionary<string, object>>();
                var clubs = reader.Select(row => 
                        new Club(Guid.Parse(row["Guid"].ToString())){ 
                            Name = (string)row["Name"], 
                            Url = (string)row["Url"]}
                        ).ToList();
                    
                return clubs;    
            }
        }
        
    }
}