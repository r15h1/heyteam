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
            Ensure.ArgumentNotNull(factory);
            this.connectionFactory = factory;
        }

        public void AddPlayer(Player player) {
            Ensure.ArgumentNotNull(player);
            using(var connection = connectionFactory.Connect())
            {
                string sql = GetInsertStatement();
                DynamicParameters p = SetupInsertParameters(player);
                connection.Open();
                connection.Execute(sql, p);                
            }
        }

        public Player GetPlayer(Guid playerId)
        {
            if (playerId.IsEmpty())
                return null;

            using(var connection = connectionFactory.Connect())
            {
                string sql = @"SELECT S.Guid AS SquadGuid, P.Guid AS PlayerGuid, 
                                    P.DateOfBirth, P.DominantFoot, P.FirstName, P.LastName, 
                                    P.Email, P.Nationality, P.SquadNumber
                                FROM Players P
                                INNER JOIN Squads S ON P.SquadId = S.SquadId
                                WHERE P.Guid = @Guid";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Guid", playerId.ToString());
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var player = reader.Select<dynamic, Player>(
                        row => new Player(Guid.Parse(row.SquadGuid.ToString()), Guid.Parse(row.PlayerGuid.ToString())) {
                            DateOfBirth = DateTime.Parse(row.DateOfBirth.ToString()), DominantFoot = char.Parse(row.DominantFoot),
                            FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
                            Nationality = row.Nationality, SquadNumber = row.SquadNumber
                        }).FirstOrDefault();
                return player;
            }
        }

        public IEnumerable<Player> GetPlayers(Guid squadId)
        {
            if (squadId.IsEmpty())
                return null;

            using(var connection = connectionFactory.Connect())
            {
                string sql = @"SELECT S.Guid AS SquadGuid, P.Guid AS PlayerGuid, 
                                    P.DateOfBirth, P.DominantFoot, P.FirstName, P.LastName, 
                                    P.Email, P.Nationality, P.SquadNumber
                                FROM Players P
                                INNER JOIN Squads S ON P.SquadId = S.SquadId
                                WHERE S.Guid = @Guid";
                DynamicParameters p = new DynamicParameters();
                p.Add("@Guid", squadId.ToString());
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var players = reader.Select<dynamic, Player>(
                        row => new Player(Guid.Parse(row.SquadGuid.ToString()), Guid.Parse(row.PlayerGuid.ToString())) {
                            DateOfBirth = row.DateOfBirth, DominantFoot = char.Parse(row.DominantFoot),
                            FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
                            Nationality = row.Nationality, SquadNumber = row.SquadNumber
                        }).ToList();
                return players;
            }
        }

		public void UpdatePlayer(Player player)
		{
			Ensure.ArgumentNotNull(player);
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