using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class AvailabilityQuery : IAvailabilityQuery {
		private readonly IDbConnectionFactory connectionFactory;

		public AvailabilityQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public IEnumerable<Availability> GetAvailabilities(AvailabilityRequest request) {
			bool includeSquadId = request.SquadId.HasValue && !request.SquadId.Value.IsEmpty();
			bool includePlayerId = request.PlayerId.HasValue && !request.PlayerId.Value.IsEmpty();
			var sql = GetSql(includeSquadId, includePlayerId);

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", request.ClubId.ToString());

			if (includeSquadId) p.Add("@SquadGuid", request.SquadId.ToString());
			if (includePlayerId) p.Add("@PlayerGuid", request.PlayerId.ToString());

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var events = reader.Select<dynamic, Availability>(
						row => new Availability(Guid.Parse(row.SquadGuid), Guid.Parse(row.EventGuid.ToString())) {
							AvailabilityStatus = (AvailabilityStatus?) row.AvailabilityId,
							DateFrom = row.DateFrom, DateTo = row.DateTo,
							Notes = row.Notes, PlayerName = row.PlayerName, SquadName = row.SquadName
						}).ToList();

				return events;
			}

		}

		private string GetSql(bool includeSquadId, bool includePlayerId) {
			return "SELECT S.Guid AS \"SquadGuid\", P.Guid AS \"PlayerGuid\", "+
					"S.Name AS \"SquadName\", P.FirstName + ' ' + P.LastName AS \"PlayerName\", " +
					"PA.AvailabilityId, PA.DateFrom, PA.DateTo, PA.Notes " +
					"FROM PlayerAvailability PA " +
					"INNER JOIN Availability A ON PA.AvailabilityId = A.AvailabilityId " +
					"INNER JOIN Players P ON P.PlayerId = PA.PlayerId " +
					"INNER JOIN Squads S ON S.SquadId = P.SquadId " +
					"INNER JOIN Clubs C ON C.ClubId= S.ClubId " +
					"WHERE C.Guid = @ClubGuid AND PA.DateTo IS NULL OR PA.DateTo >= GetDate() " +
					(includeSquadId ? "AND S.Guid = @SquadGuid " : string.Empty) +
					(includePlayerId ? "AND P.Guid = @PlayerGuid " : string.Empty);
		}
		
	}
}
