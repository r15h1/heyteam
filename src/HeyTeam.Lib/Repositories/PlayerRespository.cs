using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
    public class PlayerRepository : IPlayerRepository {

        private readonly IDbConnectionFactory connectionFactory;
        public PlayerRepository(IDbConnectionFactory factory) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }

        public void AddPlayer(Player player) {
            ThrowIf.ArgumentIsNull(player);
            using(var connection = connectionFactory.Connect())
            {
                string sql = GetInsertStatement();
                DynamicParameters p = SetupInsertParameters(player);
                connection.Open();
                connection.Execute(sql, p);                
            }
        }

		public void UpdatePlayer(Player player)
		{
			ThrowIf.ArgumentIsNull(player);
			using (var connection = connectionFactory.Connect())
			{
				string sql = GetUpdateStatement();
				DynamicParameters p = SetupUpdateParameters(player);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		private string GetInsertStatement() {
            return @"INSERT INTO PLAYERS (
                SquadId, Guid, DateOfBirth, 
                DominantFoot, FirstName, LastName, Email, Nationality, SquadNumber
            ) 
            SELECT S.SquadId, @PlayerGuid, @DateOfBirth, 
                @DominantFoot, @FirstName, @LastName, @Email, @Nationality, @SquadNumber
            FROM SQUADS S
            WHERE S.Guid = @SquadGuid";


        }

        private DynamicParameters SetupInsertParameters(Player player) {
            var p = new DynamicParameters();
            p.Add("@SquadGuid", player.SquadId.ToString());
            p.Add("@PlayerGuid", player.Guid.ToString());
            p.Add("@DateOfBirth", player.DateOfBirth);
            p.Add("@DominantFoot", player.DominantFoot.ToString());
            p.Add("@FirstName", player.FirstName);
            p.Add("@LastName", player.LastName);
            p.Add("@Email", player.Email);
            p.Add("@Nationality", player.Nationality);
			p.Add("@SquadNumber", player.SquadNumber);
			return p;
        }

		private string GetUpdateStatement()
		{
			return @"UPDATE PLAYERS SET DateOfBirth =@DateOfBirth, 
						DominantFoot = @DominantFoot,
						FirstName = @FirstName,       
						LastName = @LastName, 
						Email = @Email, 
						Nationality = @Nationality, 
						SquadNumber = @SquadNumber                 
            WHERE Guid = @PlayerGuid";
		}

		private DynamicParameters SetupUpdateParameters(Player player)
		{
			var p = new DynamicParameters();
			p.Add("@PlayerGuid", player.Guid.ToString());
			p.Add("@DateOfBirth", player.DateOfBirth);
			p.Add("@DominantFoot", player.DominantFoot.ToString());
			p.Add("@FirstName", player.FirstName);
			p.Add("@LastName", player.LastName);
			p.Add("@Email", player.Email);
			p.Add("@Nationality", player.Nationality);
			p.Add("@SquadNumber", player.SquadNumber);
			return p;
		}
	}
}