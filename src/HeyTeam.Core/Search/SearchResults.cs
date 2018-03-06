using System.Collections.Generic;

namespace HeyTeam.Core.Search {
	public class SearchResults
    {
		public SearchResults(SearchCriteria searchCriteria){
			SearchCriteria = searchCriteria;
		}

		public SearchCriteria SearchCriteria { get; }
		public int TotalHitsCount { get; set; }
		public ICollection<Hit> Hits { get; } = new List<Hit>();
		public void AddSearchResult(Hit hit) {
			if (hit != null)
				Hits.Add(hit);
		}
	}
}