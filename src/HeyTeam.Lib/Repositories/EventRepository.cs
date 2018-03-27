using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Data;
using System.Linq;

namespace HeyTeam.Lib.Repositories {
	public class EventRepository : IEventRepository {

		private readonly IDbConnectionFactory connectionFactory;
		private enum SaveAction { 
			CREATE,
			UPDATE
		}

		public EventRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddEvent(Event @event) => SaveEvent(@event, SaveAction.CREATE);

		public void UpdateEvent(Event @event) => SaveEvent(@event, SaveAction.UPDATE);

		private void SaveEvent(Event @event, SaveAction action) {
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						if (action == SaveAction.CREATE)
							CreateEvent(@event, connection, transaction);
						else if (action == SaveAction.UPDATE)
							UpdateEvent(@event, connection, transaction);

						UpdateSquadEvents(@event, connection, transaction);
						UpdateEventTrainingMaterials(@event, connection, transaction);
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		private static void CreateEvent(Event @event, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) {
			string insertEventSql = @"INSERT INTO Events(ClubId, Guid, Title, StartDate, EndDate, Location, EventTypeId) 
                                SELECT C.ClubId, @EventGuid, @Title, @StartDate, @EndDate, @Location, @EventTypeId
								FROM CLUBS C  
                                WHERE C.Guid = @ClubGuid";

			var insertEventParameters = new DynamicParameters();
			insertEventParameters.Add("@ClubGuid", @event.ClubId.ToString());
			insertEventParameters.Add("@EventGuid", @event.Guid.ToString());
			insertEventParameters.Add("@Title", @event.Title);
			insertEventParameters.Add("@StartDate", @event.StartDate);
			insertEventParameters.Add("@EndDate", @event.EndDate);
			insertEventParameters.Add("@Location", @event.Location);
			insertEventParameters.Add("@EventTypeId", (byte) @event.EventType);
			connection.Execute(insertEventSql, insertEventParameters, transaction);
		}

		private static void UpdateEvent(Event @event, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) {
			string updateEventSql = @" 
								DELETE SquadEvents WHERE EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid);
								DELETE EventTrainingMaterials WHERE EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid);
								UPDATE Events SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, Location = @Location, EventTypeId = @EventTypeId
								WHERE Guid = @EventGuid;";

			var updateParameters = new DynamicParameters();
			updateParameters.Add("@ClubGuid", @event.ClubId.ToString());
			updateParameters.Add("@EventGuid", @event.Guid.ToString());
			updateParameters.Add("@Title", @event.Title);
			updateParameters.Add("@StartDate", @event.StartDate);
			updateParameters.Add("@EndDate", @event.EndDate);
			updateParameters.Add("@Location", @event.Location);
			updateParameters.Add("@EventTypeId", (byte)@event.EventType);
			connection.Execute(updateEventSql, updateParameters, transaction);
		}

		private static void UpdateEventTrainingMaterials(Event @event, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) {
			string insertTrainingMaterialsSql = @"INSERT INTO EventTrainingMaterials (EventId, TrainingMaterialId) VALUES (										
										(SELECT EventId FROM Events WHERE Guid = @EventGuid),
										(SELECT TrainingMaterialId FROM TrainingMaterials WHERE Guid = @TrainingMaterialGuid))";
			if (@event.TrainingMaterials != null) {
				foreach (var material in @event.TrainingMaterials) {
					var materialParameters = new DynamicParameters();
					materialParameters.Add("@EventGuid", @event.Guid.ToString());
					materialParameters.Add("@TrainingMaterialGuid", material.Guid.ToString());
					connection.Execute(insertTrainingMaterialsSql, materialParameters, transaction);
				}
			}
		}

		private static void UpdateSquadEvents(Event @event, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) {
			string assignEventToSquadsSql = @"INSERT INTO SquadEvents (SquadId, EventId) VALUES (
										(SELECT SquadId FROM Squads WHERE Guid = @SquadGuid),
										(SELECT EventId FROM Events WHERE Guid = @EventGuid));";

			foreach (var squad in @event.Squads) {
				var eventSquadParameters = new DynamicParameters();
				eventSquadParameters.Add("@EventGuid", @event.Guid.ToString());
				eventSquadParameters.Add("@SquadGuid", squad.Guid);
				connection.Execute(assignEventToSquadsSql, eventSquadParameters, transaction);
			}
		}

		public void DeleteEvent(Guid clubId, Guid eventId) {
			using (var connection = connectionFactory.Connect()) {
				string deleteSql = @"UPDATE Events SET Deleted = 1, DeletedOn = GetDate() WHERE Guid = @EventGuid;";

				var deleteParameters = new DynamicParameters();
				deleteParameters.Add("@EventGuid", eventId.ToString());

				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(deleteSql, deleteParameters, transaction);
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		public void UpdateAttendance(Guid squadId, Guid eventId, Guid playerId, Attendance? attendance) {
			string sql = @"IF @AttendanceId IS NULL  
						 BEGIN
							DELETE EventAttendance 
							WHERE SquadId = (SELECT SquadId FROM Squads WHERE Guid = @SquadGuid)
							AND EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid) 
							AND PlayerId = (SELECT PlayerId FROM Players WHERE GUID = @PlayerGuid); 
						END
						ELSE IF @AttendanceId IS NOT NULL 
						BEGIN
							MERGE EventAttendance Target
							USING (
								VALUES((SELECT SquadId FROM Squads WHERE Guid = @SquadGuid),
										(SELECT EventId FROM Events WHERE Guid = @EventGuid),
										(SELECT PlayerId FROM Players WHERE GUID = @PlayerGuid)
								)
							)
							AS Source (SquadId, EventId, PlayerId)
							ON Target.SquadId = Source.SquadId AND Target.EventId = Source.EventId AND Target.PlayerId = Source.PlayerId
							WHEN MATCHED THEN
								UPDATE SET AttendanceId = @AttendanceId, TimeLogged = CASE WHEN @AttendanceId = 2 THEN NULL ELSE TimeLogged END
							WHEN NOT MATCHED BY Target THEN
								INSERT (SquadId, EventId, PlayerId, AttendanceId)
								VALUES(SquadId, EventId, PlayerId, @AttendanceId) ;
						END";

			var parameters = new DynamicParameters();
			parameters.Add("@SquadGuid", squadId.ToString());
			parameters.Add("@EventGuid", eventId.ToString());
			parameters.Add("@PlayerGuid", playerId.ToString());
			parameters.Add("@AttendanceId", (attendance.HasValue ? (short?) attendance : null)) ;
			
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(sql, parameters, transaction);
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		public void AddEventReview(NewEventReviewRequest request) {
            var sql = @"INSERT INTO EventReviews 
                            (Guid, EventId, CoachId, LastReviewedDate, Successes, Opportunities, DifferentNextTime)
                      VALUES (
		                    @EventReviewGuid, 
		                    (SELECT EventId FROM Events WHERE Guid = @EventGuid),
		                    (SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid),
		                    GetDate(), @Successes, @Opportunities, @DifferentNextTime
	                    )

	                    INSERT INTO EventReviewSquads (EventReviewId, SquadId)
	                    SELECT (SELECT EventReviewId FROM EventReviews WHERE Guid = @EventReviewGuid), SquadId 
                        FROM Squads WHERE Guid in @SquadGuids
	                    ";
            var parameters = new DynamicParameters();
            parameters.Add("@EventGuid", request.EventId.ToString());
            parameters.Add("@EventReviewGuid", Guid.NewGuid().ToString());
            parameters.Add("@CoachGuid", request.CoachId.ToString());
            parameters.Add("@Successes", request.Successes);
            parameters.Add("@Opportunities", request.Opportunities);
            parameters.Add("@DifferentNextTime", request.DifferentNextTime);
            parameters.Add("@SquadGuids", request.Squads.ToArray());

            using (var connection = connectionFactory.Connect()) {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) {
                    try {
                        connection.Execute(sql, parameters, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex) {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

        }

		public void SaveEventReport(EventReport report) {
			var sql = @"DELETE FROM EventReports WHERE EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid);			
					  INSERT INTO EventReports
                            (EventId, Report)
                      VALUES (		                    
		                    (SELECT EventId FROM Events WHERE Guid = @EventGuid),
		                    @ReportXml
	                    )";
			var parameters = new DynamicParameters();
			parameters.Add("@EventGuid", report.EventId.ToString());
			parameters.Add("@ReportXml", report.Report);

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(sql, parameters, transaction);
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		public void UpdateTimeLog(Guid squadId, Guid eventId, Guid playerId, short? timeLogged) {
			string sql = @"UPDATE EventAttendance SET TimeLogged = @TimeLogged 
							WHERE SquadId = (SELECT SquadId FROM Squads WHERE Guid = @SquadGuid) AND 
							EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid) AND
							PlayerId = (SELECT PlayerId FROM Players WHERE GUID = @PlayerGuid)";

			var parameters = new DynamicParameters();
			parameters.Add("@SquadGuid", squadId.ToString());
			parameters.Add("@EventGuid", eventId.ToString());
			parameters.Add("@PlayerGuid", playerId.ToString());
			parameters.Add("@TimeLogged", (timeLogged.HasValue ? (short?)timeLogged : null));

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				connection.Execute(sql, parameters);				
			}
		}
	}
}
