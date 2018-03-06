using System;

namespace HeyTeam.Core.Search {
	public class SearchCriteria {
		public SearchCriteria(Guid clubId) {
			ClubId = clubId;
		}

		public string SearchEntity { get; set; }
		public string SearchTerm { get; set; }
		public int Page { get; set; } = 1;
		public int Limit { get; set; } = 10;
		public Guid ClubId { get; }
	}
}
