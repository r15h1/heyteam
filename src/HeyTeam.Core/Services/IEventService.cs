using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services {

	public interface IEventService {
		Response CreateEvent(EventSetupRequest request);
		Response UpdateEvent(EventSetupRequest request);
		Response DeleteEvent(EventDeleteRequest request);
		Response UpdateAttendance(EventAttendanceRequest request);
	}

	public class EventSetupRequest {
		public Guid? EventId { get; set; }
		public Guid ClubId { get; set; }
		public string Title { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public IEnumerable<Guid> TrainingMaterials { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Location { get; set; }
	}

	public class EventDeleteRequest {
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
	}

	public class EventAttendanceRequest{
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public Guid SquadId { get; set; }
		public Guid PlayerId { get; set; }
		public Attendance? Attendance{ get; set; }
	}
}