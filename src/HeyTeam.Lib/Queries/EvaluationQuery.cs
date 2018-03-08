using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Models;
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

		public IEnumerable<PlayerReportCard> GetPlayerReportCards(Guid clubId, Guid termId, Guid squadId) {
			string sql = @"SELECT P.FirstName, P.LastName, P.SquadNumber, P.Guid AS PlayerGuid, 
						PRC.[Guid] AS PlayerReportCardGuid
						FROM Players P
						INNER JOIN Squads S ON P.SquadId = S.SquadId
						INNER JOIN Clubs C ON C.ClubId = S.ClubId
						LEFT JOIN[PlayerReportCards] PRC ON P.PlayerId = PRC.PlayerId
						LEFT JOIN EvaluationTerms ET ON ET.TermId = PRC.TermId AND(ET.Deleted IS NULL OR ET.Deleted = 0) AND ET.Guid = @TermGuid
						WHERE C.Guid = @ClubGuid AND S.Guid = @SquadGuid";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());
			p.Add("@SquadGuid", squadId.ToString());
			p.Add("@TermGuid", termId.ToString());

			using (var connection = factory.Connect()) {
				connection.Open();
				var reportCards = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, PlayerReportCard>(
						row => new PlayerReportCard(
								Guid.Parse(row.PlayerGuid.ToString()),
								(row.PlayerReportCardGuid != null ? Guid.Parse(row.PlayerReportCardGuid.ToString()) : null )) 
						{
							PlayerName = $"{row.FirstName} {row.LastName}",
							SquadNumber = row.SquadNumber
						}).ToList();

				return reportCards;
			}
		}

		public Term GetTerm(Guid termId) {
            string sql = $@"SELECT E.Guid AS TermGuid, C.Guid AS ClubGuid, E.Title, E.TermStatusId,
                                E.StartDate, E.EndDate
                            FROM EvaluationTerms E   
                            INNER JOIN Clubs C ON E.ClubId = C.ClubId
                            WHERE (E.Deleted IS NULL OR E.Deleted = 0) AND E.Guid = @TermGuid;";

            DynamicParameters p = new DynamicParameters();
            p.Add("@TermGuid", termId.ToString());           

            using (var connection = factory.Connect())
            {
                connection.Open();
                var term = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, Term>(
                        row => new Term(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.TermGuid.ToString()))
                        {
                            EndDate = row.EndDate,
                            StartDate = row.StartDate,
                            TermStatus = (TermStatus)row.TermStatusId,
                            Title = row.Title
                        }).SingleOrDefault();

                return term;
            }
        }

		public IEnumerable<Term> GetTerms(Guid clubId, DateTime? startDate = null, DateTime? endDate = null, TermStatus? status = null) {
			string sql = $@"SELECT E.Guid AS TermGuid, C.Guid AS ClubGuid, E.Title, E.TermStatusId,
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
