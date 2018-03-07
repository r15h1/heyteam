namespace HeyTeam.Core.Search {
	public interface ISearchEngine
    {
		SearchResults Search(SearchCriteria criteria);
		void UpdateCache();
    }	
}
