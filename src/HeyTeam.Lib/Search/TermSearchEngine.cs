using System;
using System.Collections.Generic;
using System.Linq;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Search;
using Microsoft.Extensions.Caching.Memory;

namespace HeyTeam.Lib.Search {
	public class TermSearchEngine : ITermSearchEngine {
		private readonly IMemoryCache cache;
		private readonly IEvaluationQuery evaluationQuery;
		private readonly object cacheKey;

		public TermSearchEngine(Club club, IMemoryCache cache, IEvaluationQuery evaluationQuery) {
			Club = club;
			this.cache = cache;
			this.evaluationQuery = evaluationQuery;
			this.cacheKey = new { CacheKey = CacheKeys.TERM_LIST, ClubId = club.Guid };
		}

		public Club Club { get; }

		public SearchResults Search(SearchCriteria criteria) {
			IEnumerable<Term> terms = null;
			if (!cache.TryGetValue<IEnumerable<Term>>(cacheKey, out terms)) {
				UpdateCache(Club.Guid);
				terms = cache.Get<IEnumerable<Term>>(cacheKey);
			}

			var hits = terms?.Where(t => t.Title.ToLowerInvariant().Contains(criteria.SearchTerm.ToLowerInvariant()))
				.Skip((criteria.Page - 1) * criteria.Limit)
				.Take(criteria.Limit)
				.Select(t => new TermHit { Id = t.Guid, Text = t.Title, Status = t.TermStatus.ToString() });

			return new SearchResults(criteria, hits) {
				DocumentsFound = terms?.Where(t => t.Title.ToLowerInvariant().Contains(criteria.SearchTerm.ToLowerInvariant())).Count() ?? 0
			};
		}

		private void UpdateCache(Guid clubId) {
			var terms = evaluationQuery.GetTerms(clubId);
			cache.Remove(CacheKeys.TERM_LIST);
			MemoryCacheEntryOptions options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(10) };
			cache.Set(cacheKey, terms, options);
		}

		public void UpdateCache() {
			UpdateCache(Club.Guid);
		}
	}
}