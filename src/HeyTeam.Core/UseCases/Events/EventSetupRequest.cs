using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases.Events {
	public class EventSetupRequest {
		public Guid ClubId { get; set; }
		public string Title { get; set; }
		public IEnumerable<Guid> Squads { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndTime { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Province { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
	}
}