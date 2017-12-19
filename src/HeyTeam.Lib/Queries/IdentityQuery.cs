using HeyTeam.Core.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using HeyTeam.Core.Identity;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using Dapper;
using System.Linq;
using HeyTeam.Core.Models;

namespace HeyTeam.Lib.Queries
{
	public class IdentityQuery : IIdentityQuery {
		private readonly IDbConnectionFactory connectionFactory;

		public IdentityQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public User GetUserByEmail(Guid clubId, string email) {
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetUsers(Guid clubId) {
			string sql = @"SELECT M.*, U.Id AS UserId, R.Name AS Role, 
							CAST (CASE WHEN U.LockoutEnd IS NOT NULL AND U.LockoutEnd > GetUTCDate() THEN 1 ELSE 0 END AS Bit) AS AccountLocked,
							U.EmailConfirmed FROM
							(SELECT P.Guid, P.FirstName, P.LastName, P.Email, P.DateOfBirth, 'Player' AS Membership
							FROM Players P
							INNER JOIN Squads S ON P.SquadId = S.SquadId
							INNER JOIN Clubs C ON C.ClubId = S.ClubId
							WHERE C.Guid = @ClubGuid
								UNION ALL
							SELECT CO.Guid, CO.FirstName, CO.LastName, CO.Email, CO.DateOfBirth, 'Coach' AS Membership
							FROM Coaches CO
							INNER JOIN Clubs CL ON CO.ClubId = CL.ClubId
							WHERE CL.Guid = @ClubGuid) M
							LEFT JOIN AspNetUsers U ON M.Email = U.Email
							LEFT JOIN AspNetUserRoles UR ON U.Id = UR.UserId
							LEFT JOIN AspNetRoles R ON UR.RoleId = R.Id
						ORDER BY M.FirstName, M.LastName, M.Email";

			var p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var users = reader.Select<dynamic, User>(row => new User(row.UserId) {
						Email = row.Email,
						Roles = row.Roles,
						Name = $"{row.FirstName} {row.LastName}",
						AccountLocked = row.AccountLocked,
						EmailConfirmed = row.EmailConfirmed,
				}).ToList();

				return users;
			}
		}
	}
}
