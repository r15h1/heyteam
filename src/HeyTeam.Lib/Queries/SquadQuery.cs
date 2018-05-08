using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Repositories {
	public class SquadQuery : ISquadQuery {
        private readonly IDbConnectionFactory connectionFactory;
		private readonly IMemberQuery memberQuery;

		public SquadQuery(IDbConnectionFactory factory, IMemberQuery memberQuery) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
			this.memberQuery = memberQuery;
		}

		public (Squad Squad, IEnumerable<Player> Players, Coach Coach) GetFullSquadDetails(Guid squadId) {
			var squad = GetSquad(squadId);
			var players = memberQuery.GetPlayers(squadId);
			var coach = memberQuery.GetSquadCoaches(squadId);

			return (squad, players, coach.FirstOrDefault());
		}

        public IEnumerable<Squad> GetMemberSquads(Guid memberId, Membership membership)
        {
            using (var connection = connectionFactory.Connect())
            {
                string sql = $@"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name 
                                FROM Squads S 
                                INNER JOIN Clubs C ON C.ClubId = S.ClubId 
                                { (membership == Membership.Coach 
                                                    ? "INNER JOIN SquadCoaches SC ON SC.SquadId = S.SquadId AND SC.CoachId = (SELECT CoachId FROM Coaches WHERE Guid = @MemberGuid AND (Deleted IS NULL OR Deleted = 0))"
													: "INNER JOIN Players P ON P.SquadId = S.SquadId AND P.Playerid = (SELECT PlayerId FROM Players WHERE Guid = @MemberGuid AND (Deleted IS NULL OR Deleted = 0))")
                                }";

                var p = new DynamicParameters();
                p.Add("@MemberGuid", memberId.ToString());
                connection.Open();

                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var squads = reader.Select<dynamic, Squad>(
                        row => new Squad(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.SquadGuid.ToString()))
                        {
                            Name = row.Name
                        }).ToList();

                return squads;
            }
        }

        public Squad GetSquad(Guid squadId) {
            using (var connection = connectionFactory.Connect()) {
                string sql = @"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name 
                                FROM Squads S 
                                INNER JOIN Clubs C ON C.ClubId = S.ClubId
                                WHERE S.Guid = @SquadGuid";  

                var p = new DynamicParameters();
                p.Add("@SquadGuid", squadId.ToString());
                connection.Open();

                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var squad = reader.Select<dynamic, Squad>(
                        row => new Squad(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.SquadGuid.ToString())) {
                            Name = row.Name
                        }).FirstOrDefault();

                return squad;                
            }
        }

		public IEnumerable<Squad> GetSquads(Guid clubId) {

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name 
                                FROM Squads S 
                                INNER JOIN Clubs C ON C.ClubId = S.ClubId
                                WHERE C.Guid = @ClubGuid";

				var p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();

				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, Squad>(
						row => new Squad(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.SquadGuid.ToString())) {
							Name = row.Name
						}).ToList();
			}
		}
    }
}