using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
	public class EventRepository : IEventRepository {

		private readonly IDbConnectionFactory connectionFactory;

		public EventRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddEvent(Event @event) {
			using (var connection = connectionFactory.Connect()) {
				string sql = @"INSERT INTO EVENTS(ClubId, Guid, Title, StartDate, EndDate, Location) 
                                SELECT C.ClubId, @EventGuid, @Title, @StartDate, @EndDate, @Location
								FROM CLUBS C  
                                WHERE C.Guid = @ClubGuid";

				var p = new DynamicParameters();				
				p.Add("@ClubGuid", @event.ClubId.ToString());
				p.Add("@EventGuid", @event.Guid.ToString());
				p.Add("@Title", @event.Title);
				p.Add("@StartDate", @event.StartDate);
				p.Add("@EndDate", @event.EndDate);
				p.Add("@Location", @event.Location);
				connection.Open();
				connection.Execute(sql, p);
			}
		}
	}
}
