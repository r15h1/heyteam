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
	}
}
