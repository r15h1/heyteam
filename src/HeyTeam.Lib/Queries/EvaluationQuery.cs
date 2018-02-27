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

		public IEnumerable<Term> GetTerms(Guid clubId) {
            if (clubId.IsEmpty())
                return null;

            string sql = @"SELECT E.Guid AS TermGuid, E.ClubId AS ClubGuid, E.Title, E.TermStatusId,
                                E.MonthStart, E.YearStart, E.MonthEnd, E.YearEnd
                            FROM EvaluationTerms E
                            INNER JOIN Clubs C ON E.ClubId = C.ClubId AND C.Guid = @ClubGuid
                            WHERE E.Deleted IS NULL OR E.Deleted = 0";
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", clubId.ToString());

            using (var connection = factory.Connect())
            {
                
                connection.Open();
                var terms = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, Term>(
                        row => new Term(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.TermGuid.ToString()))
                        {
                            PeriodEnd = new TermPeriod { Month = row.MonthEnd, Year = row.YearEnd },
                            PeriodStart = new TermPeriod { Month = row.MonthStart, Year = row.YearStart },
                            TermStatus = (TermStatus) row.TermStatusId,
                            Title = row.Title
                        }).ToList();

                return terms;
            }
		}
	}
}
