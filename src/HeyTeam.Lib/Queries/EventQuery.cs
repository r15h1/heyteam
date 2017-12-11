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
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND E.Guid = @EventGuid
								WHERE E.Deleted IS NULL OR E.Deleted = 0;
								
								SELECT S.Guid AS SquadGuid, S.Name
								FROM SquadEvents SE
								INNER JOIN Events E ON SE.EventId = E.EventId AND E.Guid = @EventGuid 
								INNER JOIN Squads S ON SE.SquadId = S.SquadId
								WHERE E.Deleted IS NULL OR E.Deleted = 0;

								SELECT T.Guid AS TrainingMaterialGuid, T.Title, T.ContentType, T.ThumbnailUrl, T.Url, T.ExternalId, T.Description
								FROM EventTrainingMaterials ETM
								INNER JOIN Events E ON ETM.EventId = E.EventId AND E.Guid = @EventGuid
								INNER JOIN TrainingMaterials T ON ETM.TrainingMaterialId = T.TrainingMaterialId
								WHERE E.Deleted IS NULL OR E.Deleted = 0;";

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

				if (@event != null)
					@event.TrainingMaterials = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, TrainingMaterial>(
						row => new TrainingMaterial(@event.ClubId, Guid.Parse(row.TrainingMaterialGuid.ToString())) {
							Title = row.Title,
							ContentType = row.ContentType,
							Description = row.Description,
							ExternalId = row.ExternalId,
							ThumbnailUrl = row.ThumbnailUrl,
							Url = row.Url
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
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
								WHERE E.StartDate >= GetDate() AND (E.Deleted IS NULL OR E.Deleted = 0);
								
								SELECT E.Guid AS EventGuid, S.Guid AS SquadGuid, S.Name
								FROM SquadEvents SE
								INNER JOIN Events E ON SE.EventId = E.EventId
								INNER JOIN Squads S ON SE.SquadId = S.SquadId
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.ClubId = S.ClubId
								WHERE C.Guid = @ClubGuid AND E.StartDate >= GetDate()  AND (E.Deleted IS NULL OR E.Deleted = 0);
								
								SELECT E.Guid AS EventGuid, T.Guid AS TrainingMaterialGuid, T.Title, T.ContentType, T.ThumbnailUrl, T.Url, T.ExternalId, T.Description
								FROM EventTrainingMaterials ETM
								INNER JOIN Events E ON ETM.EventId = E.EventId 
								INNER JOIN TrainingMaterials T ON ETM.TrainingMaterialId = T.TrainingMaterialId
								WHERE (T.Deleted IS NULL OR T.Deleted = 0) AND 
								E.EventId IN (SELECT E.EventId FROM Events E
												INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
												WHERE E.StartDate >= GetDate() AND (E.Deleted IS NULL OR E.Deleted = 0));";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var events = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, Event>(
						row => new Event(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title
						}).ToList();

				var squads = reader.Read().Cast<dynamic>();
				var trainingMaterials = reader.Read().Cast<dynamic>();

				foreach (var @event in events) { 
					@event.Squads = squads.Where(r => r.EventGuid == @event.Guid)
										.Select<dynamic, Squad>(row => new Squad(@event.ClubId, Guid.Parse(row.SquadGuid.ToString())) {
											Name = row.Name
										}).ToList();

					@event.TrainingMaterials = trainingMaterials.Where(r => r.EventGuid == @event.Guid)
												.Select<dynamic, TrainingMaterial>(row => new TrainingMaterial(@event.ClubId, Guid.Parse(row.EventGuid.ToString())) {
													ContentType = row.ContentType, Description = row.Description, ExternalId = row.ExternalId, 
													ThumbnailUrl = row.ThumbnailUrl, Title = row.Title, Url = row.Url
												}).ToList();
				}
				
				return events;
			}
		}
	}
}