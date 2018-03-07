using HeyTeam.Core.Models;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEvaluationQuery {
		IEnumerable<PlayerReportCard> GetPlayerReportCards(Guid clubId, Guid termId, Guid squadId);
		IEnumerable<Term> GetTerms(Guid clubId, DateTime? startDate = null, DateTime? endDate = null, TermStatus? status = null);
		Term GetTerm(Guid termId);
    }
}
