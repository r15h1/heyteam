using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core.Models;
using System.Xml;
using HeyTeam.Core.Models.Mini;

namespace HeyTeam.Lib.Queries {
	public class EventQuery : IEventQuery {

		private readonly IDbConnectionFactory connectionFactory;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;

		public EventQuery(IDbConnectionFactory factory, ISquadQuery squadQuery, IMemberQuery memberQuery) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
		}

		public Event GetEvent(Guid eventId) {
			if (eventId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, E.StartDate, E.EndDate, E.Location, E.EventTypeId
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
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title,
							EventType = (EventType) row.EventTypeId
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
				string sql = @"SELECT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, E.StartDate, E.EndDate, E.Location, E.EventTypeId
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
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title,
							EventType = (EventType)row.EventTypeId
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

		public IEnumerable<EventSummary> GetEventsSummary(Guid clubId) {
			if (clubId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT	C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, 
										E.StartDate, E.EndDate, E.Location, E.EventTypeId,
										(SELECT COUNT(1) FROM EventTrainingMaterials ETM 
											INNER JOIN TrainingMaterials T ON ETM.TrainingMaterialId = T.TrainingMaterialId
											WHERE ETM.EventId = E.EventId AND (T.Deleted IS NULL OR T.Deleted = 0)
										) AS TrainingMaterialCount,

										(SELECT STUFF(
												(SELECT ', ' + Name FROM (SELECT S.Name AS Name FROM Squads S
												INNER JOIN SquadEvents SE ON SE.SquadId = S.SquadId
												WHERE SE.EventId = E.EventId)SQ ORDER BY Name FOR XML PATH (''))
											,1,1,'')
										) AS Squads
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
								WHERE E.StartDate >= GetDate() AND (E.Deleted IS NULL OR E.Deleted = 0);";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var events = reader.Select<dynamic, EventSummary>(
						row => new EventSummary(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, Title = row.Title, 
							Squads = row.Squads, TrainingMaterialsCount = row.TrainingMaterialCount,
							EventType = (EventType)row.EventTypeId
						}).ToList();

				return events;
			}
		}

		public IEnumerable<EventSummary> GetEventsSummary(EventsRequest request) {
			if (request == null || request.ClubId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT DISTINCT C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, 
										E.StartDate, E.EndDate, E.Location, E.EventTypeId,
										(SELECT COUNT(1) FROM EventTrainingMaterials ETM 
											INNER JOIN TrainingMaterials T ON ETM.TrainingMaterialId = T.TrainingMaterialId
											WHERE ETM.EventId = E.EventId AND (T.Deleted IS NULL OR T.Deleted = 0)
										) AS TrainingMaterialCount,

										(SELECT STUFF(
												(SELECT ', ' + Name FROM (SELECT S.Name AS Name FROM Squads S
												INNER JOIN SquadEvents SE ON SE.SquadId = S.SquadId
												WHERE SE.EventId = E.EventId)SQ ORDER BY Name FOR XML PATH (''))
											,1,1,'')
										) AS Squads,
										(SELECT TOP 1 AttendanceId 
											FROM EventAttendance EA 
											INNER JOIN Players P ON EA.PlayerId = P.PlayerId AND P.Guid = @PlayerId
											WHERE EA.EventId = E.EventId AND (P.Deleted IS NULL OR P.Deleted = 0)
										) AS AttendanceId
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
								INNER JOIN SquadEvents SE ON SE.EventId = E.EventId
								INNER JOIN Squads S ON S.SquadId = SE.SquadId
								WHERE (E.Deleted IS NULL OR E.Deleted = 0)
									AND MONTH(E.StartDate) = @Month AND YEAR(E.StartDate) = @Year
									AND (@SquadId IS NULL OR S.Guid = @SquadId)
								;";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", request.ClubId.ToString());
				p.Add("@Month", request.Month);
				p.Add("@Year", request.Year);

				if(request.SquadId.IsEmpty()) {
					p.Add("@SquadId", null);
				} else {                    
					p.Add("@SquadId", request.SquadId);
				}

				if (request.PlayerId.IsEmpty()) {
					p.Add("@PlayerId", null);
				} else {
					p.Add("@PlayerId", request.PlayerId);
				}

				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var events = reader.Select<dynamic, EventSummary>(
						row => new EventSummary(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate, 
							Title = row.Title, Squads = row.Squads, TrainingMaterialsCount = row.TrainingMaterialCount,
							Attendance = (Attendance?) row.AttendanceId, EventType = (EventType)row.EventTypeId
						}).ToList();

				return events;
			}
		}

		public IEnumerable<EventPlayer> GetPlayersByEvent(Guid eventId) {
			if (eventId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT S.Guid AS SquadGuid, E.Guid AS EventGuid, P.Guid AS PlayerGuid, 
									P.DateOfBirth, P.DominantFoot, P.FirstName, P.LastName, 
									P.Email, P.Nationality, P.SquadNumber, S.Name AS SquadName,
									EA.AttendanceId, EA.TimeLogged, EA.Feedback
								FROM Players P
								INNER JOIN Squads S ON P.SquadId = S.SquadId
								INNER JOIN SquadEvents SE ON S.SquadId = SE.SquadId
								INNER JOIN Events E ON E.EventId = SE.EventId
								LEFT JOIN EventAttendance EA ON SE.SquadId = EA.SquadId AND EA.EventId = SE.EventId AND EA.PlayerId = P.PlayerId AND EA.EventId = E.EventId						
								WHERE E.Guid = @Guid AND (P.Deleted IS NULL OR P.Deleted = 0)
								ORDER BY S.Name, P.FirstName, P.LastName";
				DynamicParameters p = new DynamicParameters();
				p.Add("@Guid", eventId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var players = reader.Select<dynamic, EventPlayer>(
						row => new EventPlayer(Guid.Parse(row.SquadGuid.ToString()), Guid.Parse(row.EventGuid.ToString()), Guid.Parse(row.PlayerGuid.ToString())) {
							PlayerName = $"{row.FirstName} {row.LastName}", SquadNumber = row.SquadNumber, SquadName = row.SquadName,
							Attendance = (Attendance?) row.AttendanceId, TimeLogged = row.TimeLogged, Feedback = row.Feedback
						}).ToList();
				return players;
			}
		}

		public IEnumerable<EventReview> GetEventReviews(Guid eventId) {
			string sql = @" SELECT E.Guid AS EventGuid, ER.Guid AS EventReviewGuid, CO.Guid AS CoachGuid,
								ER.LastReviewedDate, ER.Successes, ER.Opportunities, ER.DifferentNextTime
							FROM EventReviews ER
							INNER JOIN Events E ON E.EventId = ER.EventId
							INNER JOIN Coaches CO ON ER.CoachId = CO.CoachId							
							WHERE E.Guid = @EventGuid AND (E.Deleted IS NULL OR E.Deleted = 0)

							SELECT ER.Guid AS EventReviewGuid, S.Guid AS SquadGuid
							FROM EventReviews ER
							INNER JOIN Events E ON E.EventId = ER.EventId
							INNER JOIN EventReviewSquads ERS ON ER.EventReviewId = ERS.EventReviewId
							INNER JOIN Squads S ON ERS.SquadId = S.SquadId
							WHERE E.Guid = @EventGuid AND (E.Deleted IS NULL OR E.Deleted = 0)";

			DynamicParameters p = new DynamicParameters();
			p.Add("@EventGuid", eventId.ToString());
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var eventReviews = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, EventReview>(
					row => new EventReview(Guid.Parse(row.EventGuid.ToString()), Guid.Parse(row.EventReviewGuid.ToString())) {
							Coach = memberQuery.GetCoach(Guid.Parse(row.CoachGuid.ToString())),
							DifferentNextTime = row.DifferentNextTime,
							LastReviewedOn = row.LastReviewedDate, Opportunities = row.Opportunities, Successes = row.Successes
					}).ToList();

				var squads = reader.Read().Cast<dynamic>().ToList();

				foreach (var review in eventReviews) {
					var squadGuids = squads.Where(r => r.EventReviewGuid == review.EventReviewId)
										.Select<dynamic, Guid>(row => Guid.Parse(row.SquadGuid.ToString())).ToList();

					foreach (var guid in squadGuids)
						review.Squads.Add(squadQuery.GetSquad(guid));
				}

				return eventReviews;
			}
		}

		public IEnumerable<Squad> GetUnReviewedSquads(Guid eventId) {
			string sql = @" SELECT S.Guid AS SquadGuid
							FROM SquadEvents SE 
							INNER JOIN Events E ON SE.EventId = E.EventId 
							INNER JOIN Squads S ON S.SquadId = SE.SquadId 
							WHERE (E.Deleted IS NULL OR E.Deleted = 0) AND E.Guid = @EventGuid 
								AND SE.SquadId NOT IN (
									SELECT ERS.SquadId
									FROM EventReviewSquads ERS 
									INNER JOIN EventReviews ER ON ERS.EventReviewId = ER.EventReviewId
									INNER JOIN Events E ON ER.EventId = E.EventId AND  E.Guid = @EventGuid
								)";

			DynamicParameters p = new DynamicParameters();
			p.Add("@EventGuid", eventId.ToString());
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>(); ;
				var squadGuids = reader.Select<dynamic, Guid>(row => Guid.Parse(row.SquadGuid.ToString())).ToList();

				List<Squad> squads = new List<Squad>();
				foreach (var guid in squadGuids)
					squads.Add(squadQuery.GetSquad(guid));

				return squads;
			}
		}

		public EventReport GetEventReport(Guid eventId) {
			if (eventId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT E.Guid, ER.Report
								FROM EventReports ER								
								INNER JOIN Events E ON ER.EventId = E.EventId
								WHERE E.Guid = @Guid";
								
				DynamicParameters p = new DynamicParameters();
				p.Add("@Guid", eventId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var report = reader.Select<dynamic, EventReport>(
						row => new EventReport(Guid.Parse(row.Guid.ToString())) {
							Report = GetXmlDocument(row.Report)
						}).FirstOrDefault();
				return report;
			}
		}

		private XmlDocument GetXmlDocument(string report) {
			var document = new XmlDocument();
			document.LoadXml(report);
			return document;
		}

		public IEnumerable<TrainingMaterialView> GetTrainingMaterialViews(Guid eventId) {
			string sql = @"SELECT T.Guid AS TrainingMaterialGuid, T.Title, T.ThumbnailUrl, T.Description
						FROM Events E
						INNER JOIN EventTrainingMaterials ET ON E.EventId = ET.EventId
						INNER JOIN TrainingMaterials T ON T.TrainingMaterialId = ET.TrainingMaterialId
						WHERE E.Guid = @EventGuid 

						SELECT DISTINCT P.Guid AS MemberGuid, P.FirstName + ' ' + P.LastName AS Name, 0 AS Membership, 
							T.Guid AS TrainingMaterialGuid
						FROM EventTrainingMaterialViews V
						INNER JOIN Events E ON E.EventId = V.EventId AND E.Guid = @EventGuid 
						INNER JOIN Players P ON P.PlayerId = V.PlayerId
						INNER JOIN TrainingMaterials T ON V.TrainingMaterialId = T.TrainingMaterialId
						WHERE (P.Deleted IS NULL OR P.Deleted = 0)
						UNION ALL
						SELECT DISTINCT C.Guid AS MemberGuid, C.FirstName + ' ' + C.LastName AS Name, 1 AS Membership,
							T.Guid AS TrainingMaterialGuid
						FROM EventTrainingMaterialViews V
						INNER JOIN Events E ON E.EventId = V.EventId AND E.Guid = @EventGuid 
						INNER JOIN Coaches C ON C.CoachId = V.CoachId
						INNER JOIN TrainingMaterials T ON V.TrainingMaterialId = T.TrainingMaterialId
						WHERE (C.Deleted IS NULL OR C.Deleted = 0)";

			DynamicParameters p = new DynamicParameters();
			p.Add("@EventGuid", eventId.ToString());
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.QueryMultiple(sql, p);
				var trainingMaterialViews = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, TrainingMaterialView>(
					row => new TrainingMaterialView(
						new MiniTrainingMaterial(Guid.Parse(row.TrainingMaterialGuid.ToString()), row.Title) { 
							ThumbnailUrl = row.ThumbnailUrl,
							Description = row.Description
						}
					)).ToList();
				
				foreach(var viewer in reader.Read().Cast<dynamic>())				
					trainingMaterialViews.SingleOrDefault(t => t.TrainingMaterial.Guid == viewer.TrainingMaterialGuid)
						?.AddViewer(new MiniMember(Guid.Parse(viewer.MemberGuid.ToString()), viewer.Name) { 
							Membership = ((Membership) viewer.Membership).ToString().ToLowerInvariant()
						});

				return trainingMaterialViews;
			}
		}

		public IEnumerable<EventSummary> GetUpcomingEvents(UpcomingEventsRequest request) {
			if (request == null)
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"SELECT DISTINCT TOP(@Limit) C.Guid AS ClubGuid, E.Guid AS EventGuid, E.Title, 
										E.StartDate, E.EndDate, E.Location, E.EventTypeId,
										(SELECT COUNT(1) FROM EventTrainingMaterials ETM 
											INNER JOIN TrainingMaterials T ON ETM.TrainingMaterialId = T.TrainingMaterialId
											WHERE ETM.EventId = E.EventId AND (T.Deleted IS NULL OR T.Deleted = 0)
										) AS TrainingMaterialCount,

										(SELECT STUFF(
												(SELECT ', ' + Name FROM (SELECT S.Name AS Name FROM Squads S
												INNER JOIN SquadEvents SE ON SE.SquadId = S.SquadId
												WHERE SE.EventId = E.EventId)SQ ORDER BY Name FOR XML PATH (''))
											,1,1,'')
										) AS Squads
								FROM Events E
								INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
								INNER JOIN SquadEvents SE ON SE.EventId = E.EventId
								INNER JOIN Squads S ON S.SquadId = SE.SquadId AND S.ClubId = C.ClubId
								INNER JOIN SquadCoaches SC ON SC.SquadId = S.SquadId
								INNER JOIN Coaches CO ON CO.CoachId = SC.CoachId AND CO.Guid = @MemberId
								WHERE (E.Deleted IS NULL OR E.Deleted = 0) AND E.StartDate >= CAST(GetDate() AS DATE);";

				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", request.ClubId.ToString());
				p.Add("@MemberId", request.MemberId.ToString());
				p.Add("@Limit", request.Limit);

				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var events = reader.Select<dynamic, EventSummary>(
						row => new EventSummary(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.EventGuid.ToString())) {
							EndDate = row.EndDate, Location = row.Location, StartDate = row.StartDate,
							Title = row.Title, Squads = row.Squads, TrainingMaterialsCount = row.TrainingMaterialCount,
							Attendance = (Attendance?)row.AttendanceId, EventType = (EventType)row.EventTypeId
						}).ToList();

				return events;
			}
		}
	}
}