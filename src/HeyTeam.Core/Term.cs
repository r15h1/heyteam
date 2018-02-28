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
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Title { get; set; }
		public TermStatus TermStatus{ get; set; }
	}

	public enum TermStatus {
		Open = 1,
		Closed = 2
	}
}
