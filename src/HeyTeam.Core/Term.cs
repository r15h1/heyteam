using System;

namespace HeyTeam.Core {
	public class Term
    {
		public Term (Guid clubId, Guid? termId){
			ClubId = clubId;
			TermId = termId ?? Guid.NewGuid();
		}

		public Guid ClubId { get; }
		public Guid? TermId { get; }
		public TermPeriod PeriodStart { get; set; } = new TermPeriod { Month = DateTime.Today.Month, Year = DateTime.Today.Year };
		public TermPeriod PeriodEnd { get; set; } = new TermPeriod { Month = DateTime.Today.Month, Year = DateTime.Today.Year };
		public string Title { get; set; }
		public TermStatus TermStatus{ get; set; }
	}

	public enum TermStatus {
		OPEN,
		CLOSED
	}

	public struct TermPeriod {
		public int Month{ get; set; }
		public int Year{ get; set; } 
	}
}
