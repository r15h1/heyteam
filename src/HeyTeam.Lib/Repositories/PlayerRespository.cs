using System;
using Dapper;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
    public class PlayerRepository : IPlayerRepository {

        private readonly IDbConnectionFactory connectionFactory;
        public PlayerRepository(IDbConnectionFactory factory) {
            Ensure.ArgumentNotNull(factory);
            this.connectionFactory = factory;
        }

        public void Add(Player player) {
            Ensure.ArgumentNotNull(player);
            using(var connection = connectionFactory.Connect())
            {
                string sql = GetInsertStatement();
                DynamicParameters p = SetupInsertParameters(player);
                connection.Open();
                connection.Execute(sql, p);                
            }
        }

        private string GetInsertStatement() {
            return @"INSERT INTO PLAYERS (
                SquadId, Guid, DateOfBirth, 
                DominantFoot, FirstName, LastName, Nationality
            ) 
            SELECT S.SquadId, @PlayerGuid, @DateOfBirth, 
                @DominantFoot, @FirstName, @LastName, @Nationality
            FROM SQUADS S
            WHERE S.Guid = @SquadGuid";


        }

        private DynamicParameters SetupInsertParameters(Player player) {
            var p = new DynamicParameters();
            p.Add("@SquadGuid", player.SquadId.ToString());
            p.Add("@PlayerGuid", player.Guid.ToString());
            p.Add("@DateOfBirth", player.DateOfBirth);
            p.Add("@DominantFoot", player.DominantFoot);
            p.Add("@FirstName", player.FirstName);
            p.Add("@LastName", player.LastName);
            p.Add("@Nationality", player.Nationality);
            return p;
        }
    }
}