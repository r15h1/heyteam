using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class EventQuery : IEventQuery {

		private readonly IDbConnectionFactory connectionFactory;

		public EventQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public Event GetEvent(Guid eventId) {
			throw new NotImplementedException();
		}

		public IEnumerable<Event> GetEvents(Guid clubId, Guid? squadId = null) {
			if (clubId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, E.StartDate, E.EndDate, E.Location
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var events = reader.Select<dynamic, Event>(
						row => new Event(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title
						}).ToList();
				return events;
			}
		}
	}
}