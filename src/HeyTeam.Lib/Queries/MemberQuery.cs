using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class MemberQuery : IMemberQuery {

        private readonly IDbConnectionFactory connectionFactory;
        public MemberQuery(IDbConnectionFactory factory) {
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
                                WHERE S.Guid = @Guid
								ORDER BY P.FirstName, P.LastName";
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

		public Coach GetCoach(Guid coachId) {
			if (coachId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT CB.Guid AS ClubGuid, CO.Guid AS CoachGuid, 
                                    CO.DateOfBirth, CO.FirstName, CO.LastName, 
                                    CO.Email, CO.Qualifications, CO.Phone
                                FROM Coaches CO
                                INNER JOIN Clubs CB ON CO.ClubId = CB.ClubId
                                WHERE CO.Guid = @Guid";
				DynamicParameters p = new DynamicParameters();
				p.Add("@Guid", coachId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var coach = reader.Select<dynamic, Coach>(
						row => new Coach(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.CoachGuid.ToString())) {
							DateOfBirth = DateTime.Parse(row.DateOfBirth.ToString()), FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
							Phone = row.Phone, Qualifications = row.Qualifications
						}).FirstOrDefault();
				return coach;
			}
		}

		public IEnumerable<Coach> GetClubCoaches(Guid clubId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT CB.Guid AS ClubGuid, CO.Guid AS CoachGuid, 
                                    CO.DateOfBirth, CO.FirstName, CO.LastName, 
                                    CO.Email, CO.Qualifications, CO.Phone
                                FROM Coaches CO
                                INNER JOIN Clubs CB ON CO.ClubId = CB.ClubId
                                WHERE CB.Guid = @ClubGuid
								ORDER BY CO.FirstName, CO.LastName";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, Coach>(
						row => (Coach)BuildCoach(row)).ToList();

			}
		}

		private static Coach BuildCoach(dynamic row) {
			return new Coach(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.CoachGuid.ToString())) {
				DateOfBirth = DateTime.Parse(row.DateOfBirth.ToString()), FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
				Phone = row.Phone, Qualifications = row.Qualifications
			};
		}

		public IEnumerable<Coach> GetSquadCoaches(Guid squadId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT CB.Guid AS ClubGuid, CO.Guid AS CoachGuid, 
                                    CO.DateOfBirth, CO.FirstName, CO.LastName, 
                                    CO.Email, CO.Qualifications, CO.Phone
                                FROM Coaches CO
                                INNER JOIN Clubs CB ON CO.ClubId = CB.ClubId
								INNER JOIN Squads SQ ON SQ.ClubId = CB.ClubId AND SQ.Guid = @SquadGuid
								INNER JOIN SquadCoaches SC ON SC.CoachId = CO.CoachId AND SC.SquadId = SQ.SquadId";
				DynamicParameters p = new DynamicParameters();
				p.Add("@SquadGuid", squadId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, Coach>(
						row => (Coach)BuildCoach(row)).ToList();

			}
		}

		public IEnumerable<Member> GetMembersByEmail(Guid clubId, string email) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"	SELECT P.Guid, P.FirstName, P.LastName, P.Email, P.DateOfBirth, 'Player' AS Membership
								FROM Players P
								INNER JOIN Squads S ON P.SquadId = S.SquadId
								INNER JOIN Clubs C ON C.ClubId = S.ClubId
								WHERE C.Guid = @Guid AND P.Email = @Email
									UNION ALL
								SELECT CO.Guid, CO.FirstName, CO.LastName, CO.Email, CO.DateOfBirth, 'Coach' AS Membership
								FROM Coaches CO
								INNER JOIN Clubs CL ON CO.ClubId = CL.ClubId
								WHERE CL.Guid = @Guid AND CO.Email = @Email";
				var p = new DynamicParameters();
				p.Add("@Guid", clubId.ToString());
				p.Add("@Email", email);
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				return reader.Select<dynamic, Member>(row =>
							new Member(Guid.Parse(row.Guid.ToString())) {
								FirstName = row.FirstName,
								LastName = row.LastName,
								DateOfBirth = row.DateOfBirth,
								Email = row.Email,
								Membership = Enum.Parse<Membership>(row.Membership),
								Phone = row.Phone
							}
						).ToList();
			}
		}

	}
}