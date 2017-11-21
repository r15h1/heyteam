using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Repositories {
	public class EventRepository : IEventRepository {

		private readonly IDbConnectionFactory connectionFactory;

		public EventRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddEvent(Event @event) {
			using (var connection = connectionFactory.Connect()) {

				string assignEventToSquadsSql = @"INSERT INTO SquadEvents (SquadId, EventId) VALUES (
										(SELECT SquadId FROM Squads WHERE Guid = @SquadGuid),
										(SELECT EventId FROM Events WHERE Guid = @EventGuid));";

				string insertEventSql = @"INSERT INTO Events(ClubId, Guid, Title, StartDate, EndDate, Location) 
                                SELECT C.ClubId, @EventGuid, @Title, @StartDate, @EndDate, @Location
								FROM CLUBS C  
                                WHERE C.Guid = @ClubGuid";

				var insertEventParameters = new DynamicParameters();
				insertEventParameters.Add("@ClubGuid", @event.ClubId.ToString());
				insertEventParameters.Add("@EventGuid", @event.Guid.ToString());
				insertEventParameters.Add("@Title", @event.Title);
				insertEventParameters.Add("@StartDate", @event.StartDate);
				insertEventParameters.Add("@EndDate", @event.EndDate);
				insertEventParameters.Add("@Location", @event.Location);

				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {						
						connection.Execute(insertEventSql, insertEventParameters,transaction);
						foreach (var squad in @event.Squads) {
							var eventSquadParameters = new DynamicParameters();
							eventSquadParameters.Add("@EventGuid", @event.Guid.ToString());
							eventSquadParameters.Add("@SquadGuid", squad.Guid);
							connection.Execute(assignEventToSquadsSql, eventSquadParameters, transaction);
						}
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		public void UpdateEvent(Event @event) {
			using (var connection = connectionFactory.Connect()) {
				string assignEventToSquadsSql = @"INSERT INTO SquadEvents (SquadId, EventId) VALUES (
										(SELECT SquadId FROM Squads WHERE Guid = @SquadGuid),
										(SELECT EventId FROM Events WHERE Guid = @EventGuid));";

				string updateEventSql = @" DELETE SquadEvents WHERE EventId = (SELECT EventId FROM Events WHERE Guid = @EventGuid);
								UPDATE Events SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, Location = @Location
								WHERE Guid = @EventGuid;";

				var updateParameters = new DynamicParameters();
				updateParameters.Add("@ClubGuid", @event.ClubId.ToString());
				updateParameters.Add("@EventGuid", @event.Guid.ToString());
				updateParameters.Add("@Title", @event.Title);
				updateParameters.Add("@StartDate", @event.StartDate);
				updateParameters.Add("@EndDate", @event.EndDate);
				updateParameters.Add("@Location", @event.Location);

				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(updateEventSql, updateParameters, transaction);
						foreach (var squad in @event.Squads) {
							var eventSquadParameters = new DynamicParameters();
							eventSquadParameters.Add("@EventGuid", @event.Guid.ToString());
							eventSquadParameters.Add("@SquadGuid", squad.Guid.ToString());
							connection.Execute(assignEventToSquadsSql, eventSquadParameters, transaction);
						}
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}
	}
}
