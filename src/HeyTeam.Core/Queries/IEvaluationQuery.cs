using System;
using System.Collections.Generic;

namespace HeyTeam.Core.Queries {
	public interface IEvaluationQuery {
		IEnumerable<Term> GetTerms(Guid clubId);
		Term GetTerm(Guid termId);
    }
}
