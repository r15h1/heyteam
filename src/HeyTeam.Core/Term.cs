using System;

namespace HeyTeam.Core {
	public class Term
    {
		public Term (Guid clubId, Guid? termId){
			ClubId = clubId;
			Guid = termId ?? System.Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid? Guid { get; }
		public TermPeriod PeriodStart { get; set; } = new TermPeriod { Month = DateTime.Today.Month, Year = DateTime.Today.Year };
		public TermPeriod PeriodEnd { get; set; } = new TermPeriod { Month = DateTime.Today.Month, Year = DateTime.Today.Year };
		public string Title { get; set; }
		public TermStatus TermStatus{ get; set; }
	}

	public enum TermStatus {
		Open = 1,
		Closed = 2
	}

	public struct TermPeriod {
		public int Month{ get; set; }
		public int Year{ get; set; } 
	}
}
