using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Services {

	public interface IEventService {
		Response CreateEvent(EventSetupRequest request);
		Response UpdateEvent(EventSetupRequest request);
	}

	public class EventSetupRequest {
		public Guid? EventId { get; set; }
		public Guid ClubId { get; set; }
		public string Title { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Location { get; set; }
	}
}