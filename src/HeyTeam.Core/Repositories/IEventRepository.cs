using System;
using HeyTeam.Core.Models;
using HeyTeam.Core.Services;

namespace HeyTeam.Core.Repositories {
	public interface IEventRepository {
		void AddEvent(Event @event);
		void UpdateEvent(Event @event);
		void DeleteEvent(Guid clubId, Guid eventId);
		void UpdateAttendance(Guid squadId, Guid eventId, Guid playerId, Attendance? attendance);
		void AddEventReview(NewEventReviewRequest request);
		void SaveEventReport(EventReport report);
		void UpdateTimeLog(Guid squadId, Guid eventId, Guid playerId, short? timeLogged);
	}
}
