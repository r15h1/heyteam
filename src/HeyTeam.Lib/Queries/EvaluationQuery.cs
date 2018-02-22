using HeyTeam.Core;
using System;
using System.Collections.Generic;

namespace HeyTeam.Lib.Queries {
	public class EvaluationQuery : IEvaluationQuery {
		public Term GetTerm(Guid termId) {
			throw new NotImplementedException();
		}

		public IEnumerable<Term> GetTerms(Guid clubId) {
			throw new NotImplementedException();
		}
	}
}
