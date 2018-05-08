using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
	public class CoachRepository : ICoachRepository {
		private readonly IDbConnectionFactory connectionFactory;
		private readonly IMemberQuery memberQuery;

		public CoachRepository(IDbConnectionFactory factory, IMemberQuery memberQuery) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
			this.memberQuery = memberQuery;
		}

		public void AddCoach(Coach coach) {
			ThrowIf.ArgumentIsNull(coach);
			using (var connection = connectionFactory.Connect()) {
				string sql = GetInsertStatement();
				DynamicParameters p = SetupParameters(coach);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void DeleteCoach(Coach coach) {
			ThrowIf.ArgumentIsNull(coach);
			using (var connection = connectionFactory.Connect()) {
				string sql = "UPDATE Coaches SET Deleted=1, DeletedOn=GetDate() WHERE Guid = @CoachGuid";
				var p = new DynamicParameters();
				p.Add("@CoachGuid", coach.Guid.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
			memberQuery.UpdateCache();
		}

		public void UpdateCoach(Coach coach) {
			ThrowIf.ArgumentIsNull(coach);
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
	}
}