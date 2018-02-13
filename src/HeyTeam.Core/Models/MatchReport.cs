using System;

namespace HeyTeam.Core.Models {
	public class MatchReport
    {
		public Guid ClubId { get; set; }
		public Guid EventId { get; set; }
		public string Opponent { get; set; }
		public string Scorers { get; set; }
		public string CoachsRemarks { get; set; }
		public byte GoalsScored { get; set; }
		public byte GoalsConceeded { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
