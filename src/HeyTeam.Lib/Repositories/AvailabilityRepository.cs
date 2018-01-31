using Dapper;
using HeyTeam.Core.Models;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Repositories
{
	public class AvailabilityRepository : IAvailabilityRepository {
		private readonly IDbConnectionFactory connectionFactory;

		public AvailabilityRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddAvailability(NewAvailabilityRequest request) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"INSERT INTO PlayerAvailability(PlayerId, AvailabilityId, DateFrom, DateTo, Notes) 
                               SELECT PlayerId, @AvailabilityId, @DateFrom, @DateTo, @Notes 
							   FROM Players P
							   INNER JOIN Squads S ON P.SquadId = S.SquadId
							   INNER JOIN Clubs C ON S.ClubId = C.ClubId
							   WHERE P.Guid = @PlayerGuid AND C.Guid = @ClubGuid";

				var p = new DynamicParameters();
				p.Add("@ClubGuid", request.ClubId.ToString());
				p.Add("@PlayerGuid", request.PlayerId.ToString());
				p.Add("@AvailabilityId", (short) request.AvailabilityStatus);
				p.Add("@DateFrom", request.DateFrom);
				p.Add("@DateTo", request.DateTo);
				p.Add("@Notes", request.Notes);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void DeleteAvailability(DeleteAvailabilityRequest request) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"DELETE PlayerAvailability
								FROM PlayerAvailability PA
								INNER JOIN Players P ON PA.PlayerId = P.PlayerId
								INNER JOIN Squads S ON P.SquadId = S.SquadId
								INNER JOIN Clubs C ON S.ClubId = C.ClubId
								WHERE C.Guid = @ClubGuid AND P.Guid = @PlayerGuid AND PA.Guid = @AvailabilityGuid";

				var p = new DynamicParameters();
				p.Add("@ClubGuid", request.ClubId.ToString());
				p.Add("@PlayerGuid", request.PlayerId.ToString());
				p.Add("@AvailabilityGuid", request.AvailabilityId.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void UpdateAvailability(UpdateAvailabilityRequest request) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"UPDATE PlayerAvailability
								SET AvailabilityId = @AvailabilityId, DateFrom = @DateFrom, DateTo = @DateTo, Notes = @Notes
								WHERE Guid = @AvailabilityGuid";

				var p = new DynamicParameters();
				p.Add("@AvailabilityGuid", request.AvailabilityId.ToString());
				p.Add("@AvailabilityId", (short)request.AvailabilityStatus);
				p.Add("@DateFrom", request.DateFrom);
				p.Add("@DateTo", request.DateTo);
				p.Add("@Notes", request.Notes);
				connection.Open();
				connection.Execute(sql, p);
			}
		}
	}
}
