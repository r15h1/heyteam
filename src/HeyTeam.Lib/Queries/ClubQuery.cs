using Dapper;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class ClubQuery : IClubQuery
    {
		private readonly IDbConnectionFactory connectionFactory;

		public ClubQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}
		public Club GetClub(Guid clubId) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT Guid, Name, Url FROM Clubs WHERE Guid = @Guid; ";
				//SELECT S.Guid, S.Name FROM Squads S 
				//    INNER JOIN Clubs C ON S.ClubId = C.ClubId 
				//    WHERE C.Guid = @Guid;";  

				var p = new DynamicParameters();
				p.Add("@Guid", clubId.ToString());
				connection.Open();

				using (var multi = connection.QueryMultiple(sql, p)) {
					var club = multi.Read().Cast<IDictionary<string, object>>().Select(row =>
						new Club(Guid.Parse(row["Guid"].ToString())) {
							Name = (string)row["Name"],
							Url = (string)row["Url"]
						}
						).FirstOrDefault();

					//var squads = multi.Read().Cast<IDictionary<string, object>>().Select(row => 
					//    new Squad(club.Guid, Guid.Parse(row["Guid"].ToString())) {
					//    Name = (string)row["Name"]
					//}).ToList();

					//if(squads != null && squads.Count > 0)
					//    squads.ForEach((s) => club.AddSquad(s));

					return club;
				}
			}
		}

		public bool IsUrlAlreadyAssigned(string url, Guid? clubId = null) {
			using (var connection = connectionFactory.Connect()) {
				string sql = "SELECT COUNT(1) FROM CLUBS WHERE Url = @Url" + (clubId.IsEmpty() ? "" : $" AND Guid != @Guid");
				var p = new DynamicParameters();
				p.Add("@Url", url);

				if (!clubId.IsEmpty())
					p.Add("@Guid", clubId.Value.ToString());

				connection.Open();
				long count = (long)connection.ExecuteScalar(sql, p);
				return count > 0;
			}
		}

		public IEnumerable<Club> GetClubs() {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT Guid, Name, Url FROM Clubs";
				connection.Open();
				var reader = connection.Query(sql).Cast<IDictionary<string, object>>();
				var clubs = reader.Select(row =>
						new Club(Guid.Parse(row["Guid"].ToString())) {
							Name = (string)row["Name"],
							Url = (string)row["Url"]
						}
						).ToList();

				return clubs;
			}
		}
	}
}
