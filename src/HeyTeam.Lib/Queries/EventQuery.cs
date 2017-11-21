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
			if (eventId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, E.StartDate, E.EndDate, E.Location
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND E.Guid = @EventGuid;
								
								SELECT S.Guid AS SquadGuid, S.Name
								FROM SquadEvents SE
								INNER JOIN Events E ON SE.EventId = E.EventId AND E.Guid = @EventGuid
								INNER JOIN Squads S ON SE.SquadId = S.SquadId;";
				DynamicParameters p = new DynamicParameters();
				p.Add("@EventGuid", eventId.ToString());
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var @event = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, Event>(
						row => new Event(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title
						}).FirstOrDefault();
				
				if(@event != null)
					@event.Squads = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, Squad>(
						row => new Squad(@event.ClubId, Guid.Parse(row.SquadGuid.ToString())) {
							Name = row.Name
						}).ToList();

				return @event;
			}
		}

		public IEnumerable<Event> GetEvents(Guid clubId, Guid? squadId = null) {
			if (clubId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, E.StartDate, E.EndDate, E.Location
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid;
								
								SELECT E.Guid AS EventGuid, S.Guid AS SquadGuid, S.Name
								FROM SquadEvents SE
								INNER JOIN Events E ON SE.EventId = E.EventId
								INNER JOIN Squads S ON SE.SquadId = S.SquadId
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.ClubId = S.ClubId
								WHERE C.Guid = @ClubGuid";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var events = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, Event>(
						row => new Event(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title
						}).ToList();

				var squads = reader.Read().Cast<dynamic>();
				foreach (var @event in events)
					@event.Squads = squads.Where(r => r.EventGuid == @event.Guid).Select<dynamic, Squad>(row => new Squad(@event.ClubId, Guid.Parse(row.SquadGuid.ToString())) {
										Name = row.Name
									}).ToList();

				return events;
			}
		}
	}
}