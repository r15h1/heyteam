using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class MemberQuery : IMemberQuery {

        private readonly IDbConnectionFactory connectionFactory;
		private readonly IMemoryCache cache;

		public MemberQuery(IDbConnectionFactory factory, IMemoryCache cache) {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
			this.cache = cache;
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
                                WHERE CO.Guid = @Guid;

                                SELECT S.Guid AS SquadGuid
                                FROM SquadCoaches SC
                                INNER JOIN Coaches C ON SC.CoachId = C.CoachId AND C.Guid = @Guid
                                INNER JOIN Squads S ON SC.SquadId = S.SquadId;";
				DynamicParameters p = new DynamicParameters();
				p.Add("@Guid", coachId.ToString());
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var coach = reader.Read().Select<dynamic, Coach>(
						row => new Coach(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.CoachGuid.ToString())) {
							DateOfBirth = DateTime.Parse(row.DateOfBirth.ToString()), FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
							Phone = row.Phone, Qualifications = row.Qualifications
						}).SingleOrDefault();

                coach.Squads = reader.Read().Select<dynamic, Guid>(row => Guid.Parse(row.SquadGuid.ToString())).ToList();

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
				string sql = @"	SELECT P.Guid, P.FirstName, P.LastName, P.Email, P.DateOfBirth, 'Player' AS Membership,
								S.Guid AS SquadGuid
								FROM Players P
								INNER JOIN Squads S ON P.SquadId = S.SquadId
								INNER JOIN Clubs C ON C.ClubId = S.ClubId
								WHERE C.Guid = @Guid AND P.Email = @Email
									UNION ALL
								SELECT CO.Guid, CO.FirstName, CO.LastName, CO.Email, CO.DateOfBirth, 'Coach' AS Membership,
								NULL AS SquadGuid
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
								//Squads = row.SquadGuid.IsEmpty() ? new List<Guid>() : new List<Guid> { row.Guid }
							}
						).ToList();
			}
		}

		public IEnumerable<PlayerSearchResult> SearchPlayers(string searchTerm, int page = 1, int limit = 10) {						
			if (searchTerm.IsEmpty()) return null;

			IEnumerable<PlayerIndexedDocument> list = null;
			if (!cache.TryGetValue<IEnumerable<PlayerIndexedDocument>>(CacheKeys.PLAYERS_SEACH_BY_NAME, out list)) {
				UpdateCache();
				list = cache.Get<IEnumerable<PlayerIndexedDocument>>(CacheKeys.PLAYERS_SEACH_BY_NAME);
			}

			return list?.Where(t => t.SearchField.Contains(searchTerm.ToLowerInvariant()))
				.Skip((page - 1) * limit)
				.Take(limit)
				.Select(t => new PlayerSearchResult  { PlayerId = t.PlayerId, PlayerName = t.PlayerName, SquadName = t.SquadName, SquadNumber = t.SquadNumber });
		}

		public void UpdateCache() {
			var list = GetPlayerDocuments();
			cache.Remove(CacheKeys.PLAYERS_SEACH_BY_NAME);
			MemoryCacheEntryOptions options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(10) };
			cache.Set(CacheKeys.PLAYERS_SEACH_BY_NAME, list, options);
		}

		private IEnumerable<PlayerIndexedDocument> GetPlayerDocuments(){
			using (var connection = connectionFactory.Connect()) {
				string sql = "SELECT P.Guid AS \"PlayerGuid\", P.FirstName + ' ' + P.LastName AS \"PlayerName\", " +
									"P.SquadNumber AS \"SquadNumber\", S.Name AS \"SquadName\" " +
								"FROM Players P " +
								"INNER JOIN Squads S ON S.SquadId = P.SquadId " +
								"ORDER BY PlayerName ";
				DynamicParameters p = new DynamicParameters();
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var list = reader.Select<dynamic, PlayerIndexedDocument>(
						row => new PlayerIndexedDocument() {
							PlayerId = Guid.Parse(row.PlayerGuid.ToString()), PlayerName = row.PlayerName, 
							SearchField = $"{row.PlayerName.ToLowerInvariant()} {row.SquadNumber} {row.SquadName.ToLowerInvariant()}",
							SquadName = row.SquadName, SquadNumber = row.SquadNumber?.ToString()
						}).ToList();

				return list;			
			}
		}

		public IEnumerable<MiniSquad> GetMembers(IEnumerable<Guid> squads) {
			if (squads == null || squads.Count() == 0)
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT S.Name AS SquadName, S.Guid AS SquadGuid 
								FROM Squads S
								WHERE S.Guid IN @Squads
				
								SELECT S.Guid AS SquadGuid, 0 AS Membership,
									P.Guid AS MemberGuid, P.FirstName + ' ' + P.LastName AS MemberName                                    
                                FROM Players P
                                INNER JOIN Squads S ON P.SquadId = S.SquadId
                                WHERE S.Guid IN @Squads
								ORDER BY P.FirstName, P.LastName";
				DynamicParameters p = new DynamicParameters();
				p.Add("@Squads", squads.ToArray());
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var squadList = reader.Read().Select<dynamic, MiniSquad>(
					row => new MiniSquad(Guid.Parse(row.SquadGuid.ToString()), row.SquadName)
				).ToList();

				foreach(var member in reader.Read().Cast<dynamic>()){
					if (squadList.Any(s => s.Guid == member.SquadGuid))
						squadList.SingleOrDefault(s => s.Guid == member.SquadGuid).AddMember(
							new MiniMember(member.MemberGuid, member.MemberName){
								Membership = ((Membership) member.Membership).ToString()
							}
						);
				}

				return squadList;
			}
		}
	}
}