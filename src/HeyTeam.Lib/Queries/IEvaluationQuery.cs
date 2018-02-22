using HeyTeam.Core;
using System;
using System.Collections.Generic;

namespace HeyTeam.Lib.Queries {
	public interface IEvaluationQuery {
		IEnumerable<Term> GetTerms(Guid clubId);
		Term GetTerm(Guid termId);
    }
}
