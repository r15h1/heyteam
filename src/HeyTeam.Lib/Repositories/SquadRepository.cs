using System.Collections.Generic;
using HeyTeam.Core.Repositories;
using System;
using System.Linq;
using HeyTeam.Lib.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Core.Entities;
using Dapper;

namespace HeyTeam.Lib.Repositories {
    public class SquadRepository : ISquadRepository {
        private readonly IDbConnectionFactory connectionFactory;

        public SquadRepository(IDbConnectionFactory factory) {
            if (factory == null)
                throw new ArgumentNullException();

            this.connectionFactory = factory;
        }

        public void Add(Squad squad) {
            using(var connection = connectionFactory.Connect()) {
            string sql =    @"INSERT INTO SQUADS(ClubId, Guid, Name) 
                                SELECT C.ClubId, @SquadGuid, @Name FROM CLUBS C  
                                    WHERE C.Guid = @ClubGuid";  
                                
                var p = new DynamicParameters();
                p.Add("@SquadGuid", squad.Guid.ToString());
                p.Add("@Name", squad.Name);
                p.Add("@ClubGuid", squad.ClubId.ToString());
                connection.Open();
                connection.Execute(sql, p);
            }
        }

        public Squad Get(Guid squadId) {
            using (var connection = connectionFactory.Connect()) {
                string sql = @"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name 
                                FROM Squads S 
                                INNER JOIN Clubs C ON C.ClubId = S.ClubId
                                WHERE S.Guid = @SquadGuid";  

                var p = new DynamicParameters();
                p.Add("@SquadGuid", squadId);
                connection.Open();

                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var squad = reader.Select<dynamic, Squad>(
                        row => new Squad(Guid.Parse(row.ClubGuid.ToString), Guid.Parse(row.SquadGuid.ToString)) {
                            Name = row.Name
                        }).FirstOrDefault();

                return squad;                
            }
        }

        public void Update(Squad squad) {
            using(var connection = connectionFactory.Connect()) {
                string sql = @"UPDATE Squads SET Name = @Name WHERE Guid = @Guid";  
                                
                var p = new DynamicParameters();
                p.Add("@Guid", squad.Guid.ToString());
                p.Add("@Name", squad.Name);
                connection.Open();
                connection.Execute(sql, p);
            }
        }
    }
}