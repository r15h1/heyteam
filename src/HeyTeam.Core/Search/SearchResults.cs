using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Core.Search {
	public class SearchResults
    {
		public SearchResults(SearchCriteria searchCriteria, IEnumerable<Hit> hits){
			SearchCriteria = searchCriteria;
			Hits = hits ?? new List<Hit>();
		}

		public SearchCriteria SearchCriteria { get; }
		public int DocumentsFound { get; set; }
		public int DocumentsReturned { get => Hits.Count(); }
		public IEnumerable<Hit> Hits { get; }		
	}
}