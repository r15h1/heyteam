using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class CoachQuery : ICoachQuery
    {
		private readonly IDbConnectionFactory connectionFactory;

		public CoachQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
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
                                WHERE CB.Guid = @ClubGuid";
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
	}
}
