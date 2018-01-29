using System;

namespace HeyTeam.Core.Models {
	public class NewAvailabilityRequest {
		public Guid ClubId { get; set;  }
		public Guid PlayerId { get; set; }
		public AvailabilityStatus AvailabilityStatus { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public string Notes { get; set; }
	}
}
