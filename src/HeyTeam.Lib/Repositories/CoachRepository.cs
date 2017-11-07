using Dapper;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Repositories {
	public class CoachRepository : ICoachRepository {
		private readonly IDbConnectionFactory connectionFactory;

		public CoachRepository(IDbConnectionFactory factory) {
			Ensure.ArgumentNotNull(factory);
			this.connectionFactory = factory;
		}

		public void AddCoach(Coach coach) {
			Ensure.ArgumentNotNull(coach);
			using (var connection = connectionFactory.Connect()) {
				string sql = GetInsertStatement();
				DynamicParameters p = SetupParameters(coach);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void UpdateCoach(Coach coach) {
			Ensure.ArgumentNotNull(coach);
			using (var connection = connectionFactory.Connect()) {
				string sql = GetUpdateStatement();
				DynamicParameters p = SetupParameters(coach);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		private string GetInsertStatement() {
			return @"INSERT INTO COACHES (
                ClubId, Guid, DateOfBirth, 
                FirstName, LastName, Email, Phone, Qualifications
            ) 
            SELECT C.ClubId, @CoachGuid, @DateOfBirth, 
                @FirstName, @LastName, @Email, @Phone, @Qualifications
            FROM Clubs C
            WHERE C.Guid = @ClubGuid";
		}

		private string GetUpdateStatement() {
			return @"UPDATE COACHES SET
				DateOfBirth = @DateOfBirth, FirstName = @FirstName, LastName = @LastName, 
				Email = @Email, Phone = @Phone, Qualifications = @Qualifications            
            WHERE Guid = @CoachGuid";
		}

		private DynamicParameters SetupParameters(Coach coach) {
			var c = new DynamicParameters();
			c.Add("@ClubGuid", coach.ClubId.ToString());
			c.Add("@CoachGuid", coach.Guid.ToString());
			c.Add("@DateOfBirth", coach.DateOfBirth);
			c.Add("@FirstName", coach.FirstName);
			c.Add("@LastName", coach.LastName);
			c.Add("@Email", coach.Email);
			c.Add("@Phone", coach.Phone);
			c.Add("@Qualifications", coach.Qualifications);
			return c;
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

		public IEnumerable<Coach> GetCoaches(Guid clubId) {
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
						row => new Coach(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.CoachGuid.ToString())) {
							DateOfBirth = DateTime.Parse(row.DateOfBirth.ToString()), FirstName = row.FirstName, LastName = row.LastName, Email = row.Email,
							Phone = row.Phone, Qualifications = row.Qualifications
						}).ToList();

			}
		}
	}
}