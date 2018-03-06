namespace HeyTeam.Core.Search {
	public interface ISearchService
    {
		SearchResults Search(SearchCriteria criteria);
    }	
}
