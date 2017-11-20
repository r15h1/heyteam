using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using HeyTeam.Core.Queries;

namespace HeyTeam.Lib.Queries {
    public class PlayerQuery : IPlayerQuery {

        private readonly IDbConnectionFactory connectionFactory;
        public PlayerQuery(IDbConnectionFactory factory) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
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
	}
}