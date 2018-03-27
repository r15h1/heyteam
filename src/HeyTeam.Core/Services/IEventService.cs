using System;
using System.Collections.Generic;
using System.Xml;

namespace HeyTeam.Core.Services {

	public interface IEventService {
		Response CreateEvent(EventSetupRequest request);
		Response UpdateEvent(EventSetupRequest request);
		Response DeleteEvent(EventDeleteRequest request);
		Response UpdateEventAttendance(EventAttendanceRequest request);
		Response AddEventReview(NewEventReviewRequest review);
		Response UpdateEventReport(EventReportRequest request);
		T DeserializeReport<T>(XmlDocument report);
		XmlDocument SerializeReport<T>(T report);
		Response EmailEventReport(EmailReportRequest request);
		Response LogEventTime(EventTimeLogRequest request);
	}

	public class EmailReportRequest {		
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public List<string> EmailAddresses { get; set; } = new List<string>();
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
		public EventType? EventType { get; set; }
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

	public class EventTimeLogRequest {
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public Guid SquadId { get; set; }
		public Guid PlayerId { get; set; }
		public short? TimeLogged { get; set; }
	}

	public class NewEventReviewRequest {
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public Guid CoachId { get; set; }		
		public string Successes { get; set; }//What went well
		public string Opportunities { get; set; }//What did not go well
		public string DifferentNextTime { get; set; }//What could have been done differently
	}

	public class EventReportRequest {
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public string Opponent{ get; set; }
		public string Scorers { get; set; }
		public string CoachsRemarks { get; set; }
		public byte GoalsScored { get; set; }
		public byte GoalsConceeded { get; set; }
	}



}