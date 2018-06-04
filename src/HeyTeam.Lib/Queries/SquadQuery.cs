using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Models.Mini;
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
                string sql = $@"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name,
                                    (SELECT COUNT(1) FROM Players PLA WHERE PLA.SquadId = S.SquadId AND (PLA.Deleted IS NULL OR PLA.Deleted = 0)) AS NumberOfPlayers,
                                    S.YearBorn
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
                            Name = row.Name,
                            NumberOfPlayers = row.NumberOfPlayers,
                            YearBorn = row.YearBorn
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
				string sql = @"SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name, S.YearBorn,
									(SELECT COUNT(1) FROM Players P WHERE P.SquadId = S.SquadId AND (P.Deleted IS NULL OR P.Deleted = 0)) AS NumberOfPlayers
                                FROM Squads S 
                                INNER JOIN Clubs C ON C.ClubId = S.ClubId
                                WHERE C.Guid = @ClubGuid
								ORDER BY S.YearBorn DESC";

				var p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();

				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, Squad>(
						row => new Squad(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.SquadGuid.ToString())) {
							Name = row.Name,
							YearBorn = row.YearBorn,
							NumberOfPlayers = row.NumberOfPlayers
						}).ToList();
			}
		}

		public IEnumerable<SquadSummary> GetSquadSummary(Guid clubId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT	E1.SquadGuid, E1.Name, E1.YearBorn, E1.NumberOfPlayers, 
										E2.Guid AS EventGuid, E2.Location, E2.Title, E2.StartDate, E2.EventTypeId,
										ET.Description AS EventTypeDescription
								FROM (
									SELECT C.Guid AS ClubGuid, S.Guid AS SquadGuid, S.Name, S.YearBorn,
										(SELECT COUNT(1) FROM Players P WHERE P.SquadId = S.SquadId AND (P.Deleted IS NULL OR P.Deleted = 0)) AS NumberOfPlayers,
										(SELECT TOP 1 E.EventId FROM Events E 
											INNER JOIN SquadEvents SE ON E.EventId = SE.EventId AND SE.SquadId = S.SquadId											
											WHERE (E.Deleted IS NULL OR E.Deleted = 0) AND E.StartDate >= CAST (GetDate() AS Date)
											ORDER BY E.StartDate ASC
										) AS EventId
									FROM Squads S 
									INNER JOIN Clubs C ON C.ClubId = S.ClubId AND C.Guid = @ClubGuid
								) E1 
								LEFT JOIN Events E2 ON E1.EventId = E2.EventId
								LEFT JOIN EventTypes ET ON E2.EventTypeId = ET.EventTypeId;";

				var p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();

				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, SquadSummary>(
						row => new SquadSummary(Guid.Parse(row.SquadGuid.ToString()), row.Name) {							
							YearBorn = row.YearBorn,
							NumberOfPlayers = row.NumberOfPlayers,
							NextEvent = (row.EventGuid == null ? null : new MiniEvent(Guid.Parse(row.EventGuid.ToString()), row.Title){
								EventType = (EventType?)row.EventTypeId , Location = row.Location, StartDate = row.StartDate?.ToString("dd-MMM-yyyy"),
								EventTypeDescription = row.EventTypeDescription
							})
						}).ToList();
			}
		}
	}
}