using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class EvaluationQuery : IEvaluationQuery {
        private readonly IDbConnectionFactory factory;

        public EvaluationQuery(IDbConnectionFactory factory)
        {
            this.factory = factory;
        }

		public Term GetTerm(Guid termId) {
			throw new NotImplementedException();
		}

		public IEnumerable<Term> GetTerms(Guid clubId, DateTime? startDate = null, DateTime? endDate = null, TermStatus? status = null) {
			string sql = $@"SELECT E.Guid AS TermGuid, E.ClubId AS ClubGuid, E.Title, E.TermStatusId,
                                E.StartDate, E.EndDate
                            FROM EvaluationTerms E
                            INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
                            WHERE (E.Deleted IS NULL OR E.Deleted = 0) 
									{(startDate.IsEmpty() ? "" : " AND @StartDate <= E.EndDate ")} 
									{(endDate.IsEmpty() ? "" : " AND @EndDate >= E.StartDate ")}
									{(status.HasValue ? " AND TermStatusId = @TermStatus" : "")}
							;";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());

			if(startDate.HasValue) p.Add("@StartDate", startDate);
			if(endDate.HasValue) p.Add("@EndDate", endDate);
			if(status.HasValue) p.Add("@TermStatus", status);

			using (var connection = factory.Connect()) {
				connection.Open();
				var terms = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, Term>(
						row => new Term(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.TermGuid.ToString())) {
							EndDate = row.EndDate,
							StartDate = row.StartDate,
							TermStatus = (TermStatus)row.TermStatusId,
							Title = row.Title
						}).ToList();

				return terms;
			}
		}
	}
}
