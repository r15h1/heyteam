using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class EvaluationQuery : IEvaluationQuery {
        private readonly IDbConnectionFactory factory;

        public EvaluationQuery(IDbConnectionFactory factory)
        {
            this.factory = factory;
        }

        public PlayerReportCard GetPlayerReportCard(Guid clubId, Guid reportCardId)
        {
            string sql = @"SELECT P.FirstName, P.LastName, P.SquadNumber, P.Guid AS PlayerGuid, 
						PRC.[Guid] AS PlayerReportCardGuid
						FROM Players P
						INNER JOIN Squads S ON P.SquadId = S.SquadId
						INNER JOIN Clubs C ON C.ClubId = S.ClubId						
						LEFT JOIN[PlayerReportCards] PRC ON P.PlayerId = PRC.PlayerId
						WHERE C.Guid = @ClubGuid AND PRC.Guid = @ReportCardId AND (P.Deleted IS NULL OR P.Deleted = 0)";

            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", clubId.ToString());
            p.Add("@ReportCardId", reportCardId.ToString());

            using (var connection = factory.Connect())
            {
                connection.Open();
                var reportCard = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, PlayerReportCard>(
                        row => new PlayerReportCard(
                                Guid.Parse(row.PlayerGuid.ToString()),
                                (row.PlayerReportCardGuid != null ? Guid.Parse(row.PlayerReportCardGuid.ToString()) : null))
                        {
                            PlayerName = $"{row.FirstName} {row.LastName}",
                            SquadNumber = row.SquadNumber
                        }).SingleOrDefault();

                return reportCard;
            }
        }

        public PlayerReportCard GetPlayerReportCard(Guid clubId, Guid termId, Guid squadId, Guid playerId) {
			string sql = @"SELECT P.FirstName, P.LastName, P.SquadNumber, P.Guid AS PlayerGuid, 
						PRC.[Guid] AS PlayerReportCardGuid
						FROM Players P
						INNER JOIN Squads S ON P.SquadId = S.SquadId
						INNER JOIN Clubs C ON C.ClubId = S.ClubId
						INNER JOIN EvaluationTerms ET ON ET.ClubId = C.ClubId AND (ET.Deleted IS NULL OR ET.Deleted = 0) AND ET.Guid = @TermGuid
						LEFT JOIN[PlayerReportCards] PRC ON P.PlayerId = PRC.PlayerId AND PRC.TermId = ET.TermId
						WHERE C.Guid = @ClubGuid AND S.Guid = @SquadGuid AND P.Guid = @PlayerGuid AND (P.Deleted IS NULL OR P.Deleted = 0)";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());
			p.Add("@SquadGuid", squadId.ToString());
			p.Add("@TermGuid", termId.ToString());
			p.Add("@PlayerGuid", playerId.ToString());

			using (var connection = factory.Connect()) {
				connection.Open();
				var reportCard = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, PlayerReportCard>(
						row => new PlayerReportCard(
								Guid.Parse(row.PlayerGuid.ToString()),
								(row.PlayerReportCardGuid != null ? Guid.Parse(row.PlayerReportCardGuid.ToString()) : null)) {
							PlayerName = $"{row.FirstName} {row.LastName}",
							SquadNumber = row.SquadNumber
						}).SingleOrDefault();

				return reportCard;
			}
		}

        public PlayerEvaluation GetPlayerReportCardDetails(Guid clubId, Guid playerReportCardId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", clubId.ToString());
            p.Add("@PlayerReportCardGuid", playerReportCardId.ToString());

            using (var connection = factory.Connect())
            {
                connection.Open();
                var evaluation = GetEvaluationContext(connection, p);

                foreach (var headline in GetReportCardHeadlines(connection, p))
                    evaluation.ReportCard.AddHeadline(headline);

                foreach (var area in GetReportCardAreas(connection, p))
                    evaluation.ReportCard.AddArea(area);

                foreach(var skill in GetReportCardSkills(connection, p))
                    evaluation.ReportCard.AddSkill(skill);

				foreach(var facet in GetReportCardFacets(connection, p))
					evaluation.ReportCard.AddFacet(facet.Key, facet.Value);				

                return evaluation;
            }
        }

        private PlayerEvaluation GetEvaluationContext(IDbConnection connection, DynamicParameters p)
        {
            string sql = @"SELECT P.Guid AS PlayerGuid, P.FirstName, P.LastName, S.Guid AS SquadGuid, S.Name AS SquadName,
		                            PR.Guid AS PlayerReportCardGuid, C.Name AS ClubName, C.Guid AS ClubGuid, 
                                    ET.Guid AS TermGuid, ET.Title AS TermName, ET.TermStatusId AS TermStatus,
                                    PR.Guid AS PlayerReportCardGuid, RCD.Name AS ReportDesignName, RCD.Guid AS ReportDesignGuid
                            FROM PlayerReportCards PR
                            INNER JOIN ReportCardDesigns RCD ON RCD.ReportCardDesignId = PR.ReportCardDesignId
                            INNER JOIN Players P ON PR.PlayerId = P.PlayerId
                            INNER JOIN Squads S ON P.SquadId = S.SquadId
                            INNER JOIN EvaluationTerms ET ON ET.TermId = PR.TermId AND (ET.Deleted IS NULL OR ET.Deleted = 0)                            
                            INNER JOIN Clubs C ON C.ClubId = ET.ClubId                            
                            WHERE C.Guid = @ClubGuid AND PR.Guid = @PlayerReportCardGuid AND (P.Deleted IS NULL OR P.Deleted = 0)";

            var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
            var evaluation = reader.Select<dynamic, PlayerEvaluation>(
                        row => new PlayerEvaluation()
                        {
                            Club = new MiniModel (Guid.Parse(row.ClubGuid.ToString()), row.ClubName),
                            Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), $"{row.FirstName} {row.LastName}"),
                            Term = new MiniTerm(Guid.Parse(row.TermGuid.ToString()), row.TermName)
                            {
                                Status = ((TermStatus)row.TermStatus).ToString()
                            },
                            ReportCard = new ReportCard(Guid.Parse(row.PlayerReportCardGuid.ToString()))
                            {
                                Design = new MiniModel(Guid.Parse(row.ReportDesignGuid.ToString()), row.ReportDesignName)
                            }
                            
                        }).SingleOrDefault();            

            return evaluation;
        }

        private IEnumerable<MiniReportCardHeadline> GetReportCardHeadlines(IDbConnection connection, DynamicParameters p)
        {
            string sql = @"SELECT RCH.Guid AS HeadlineGuid, RCH.Title AS HeadlineTitle, RCH.SortOrder
                            FROM PlayerReportCards PR
                            INNER JOIN ReportCardDesigns RCD ON RCD.ReportCardDesignId = PR.ReportCardDesignId
                            INNER JOIN Clubs C ON RCD.ClubId = C.ClubId
                            INNER JOIN ReportCardHeadlines RCH ON RCH.ReportCardDesignId = RCD.ReportCardDesignId AND (RCH.Deleted IS NULL OR RCH.Deleted = 0)
                            WHERE C.Guid = @ClubGuid AND PR.Guid = @PlayerReportCardGuid
                            ORDER BY RCH.SortOrder, RCH.Title";

            var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
            var headlines = reader.Select<dynamic, MiniReportCardHeadline>(
                row => new MiniReportCardHeadline(Guid.Parse(row.HeadlineGuid.ToString()))
                {
                    SortOrder = row.SortOrder,
                    Title = row.HeadlineTitle
                }
            );
            return headlines;
        }

        private IEnumerable<MiniReportCardArea> GetReportCardAreas(IDbConnection connection, DynamicParameters p)
        {
            string sql = @"SELECT	RCH.Guid AS HeadlineGuid, RCA.Guid AS AreaGuid, RCA.SortOrder AS SortOrder, RCA.Title AS AreaTitle
                            FROM PlayerReportCards PR
                            INNER JOIN ReportCardDesigns RCD ON RCD.ReportCardDesignId = PR.ReportCardDesignId
                            INNER JOIN Clubs C ON RCD.ClubId = C.ClubId
                            INNER JOIN ReportCardHeadlines RCH ON RCH.ReportCardDesignId = RCD.ReportCardDesignId AND (RCH.Deleted IS NULL OR RCH.Deleted = 0)
                            INNER JOIN ReportCardAreas RCA ON RCH.ReportCardHeadlineId = RCA.ReportCardHeadlineId AND (RCA.Deleted IS NULL OR RCA.Deleted = 0)
                            WHERE C.Guid = @ClubGuid AND PR.Guid = @PlayerReportCardGuid
                            ORDER BY RCA.SortOrder, RCA.Title";

            var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
            var areas = reader.Select<dynamic, MiniReportCardArea>(
                row => new MiniReportCardArea(Guid.Parse(row.AreaGuid.ToString()), Guid.Parse(row.HeadlineGuid.ToString()))
                {
                    SortOrder = row.SortOrder,
                    Title = row.AreaTitle
                }
            );
            return areas;
        }

        private IEnumerable<MiniReportCardSkill> GetReportCardSkills(IDbConnection connection, DynamicParameters p)
        {
            string sql = @"SELECT	RCS.Guid AS SkillGuid, RCA.Guid AS AreaGuid, RCS.Title AS SkillTitle, PRG.ReportCardGradeId
                            FROM PlayerReportCards PR
                            INNER JOIN ReportCardDesigns RCD ON RCD.ReportCardDesignId = PR.ReportCardDesignId
                            INNER JOIN Clubs C ON RCD.ClubId = C.ClubId
                            INNER JOIN ReportCardHeadlines RCH ON RCH.ReportCardDesignId = RCD.ReportCardDesignId AND (RCH.Deleted IS NULL OR RCH.Deleted = 0)
                            INNER JOIN ReportCardAreas RCA ON RCH.ReportCardHeadlineId = RCA.ReportCardHeadlineId AND (RCA.Deleted IS NULL OR RCA.Deleted = 0)
                            INNER JOIN ReportCardSkills RCS ON RCS.ReportCardAreaId = RCA.ReportCardAreaId AND (RCS.Deleted IS NULL OR RCS.Deleted = 0)
                            LEFT JOIN PlayerReportCardGrades PRG ON PRG.PlayerReportCardId = PR.PlayerReportCardId AND PRG.ReportCardSkillId = RCS.ReportCardSkillId
                            WHERE C.Guid = @ClubGuid AND PR.Guid = @PlayerReportCardGuid
                            ORDER BY RCS.Title";

            var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
            var skills = reader.Select<dynamic, MiniReportCardSkill>(
                row => new MiniReportCardSkill(Guid.Parse(row.SkillGuid.ToString()), Guid.Parse(row.AreaGuid.ToString()))
                {
                    //SortOrder = row.SortOrder,
                    Title = row.SkillTitle,
                    Grade = (ReportCardGrade?)row.ReportCardGradeId
                }
            );
            return skills;
        }

		private IDictionary<string, string> GetReportCardFacets(IDbConnection connection, DynamicParameters p)
		{
			string sql = @"SELECT FacetKey, FacetValue 
							FROM PlayerReportCardFacets F
							INNER JOIN PlayerReportCards P ON P.PlayerReportCardId = F.PlayerReportCardId AND P.Guid = @PlayerReportCardGuid";

			return connection.Query(sql, p).ToDictionary (
				row => (string)row.FacetKey,
				row => (string)row.FacetValue
			);
		}

        public IEnumerable<PlayerReportCard> GetPlayerReportCards(Guid clubId, Guid termId, Guid squadId) {
			string sql = @"SELECT P.FirstName, P.LastName, P.SquadNumber, P.Guid AS PlayerGuid, 
						PRC.[Guid] AS PlayerReportCardGuid
						FROM Players P
						INNER JOIN Squads S ON P.SquadId = S.SquadId
						INNER JOIN Clubs C ON C.ClubId = S.ClubId
						INNER JOIN EvaluationTerms ET ON ET.ClubId = C.ClubId AND (ET.Deleted IS NULL OR ET.Deleted = 0) AND ET.Guid = @TermGuid
						LEFT JOIN[PlayerReportCards] PRC ON P.PlayerId = PRC.PlayerId AND PRC.TermId = ET.TermId
						WHERE C.Guid = @ClubGuid AND S.Guid = @SquadGuid AND (P.Deleted IS NULL OR P.Deleted = 0)";

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

		public IEnumerable<MiniReportCard> GetPlayerReportCards(Guid clubId, Guid playerId) {
			string sql = @"SELECT T.StartDate, T.EndDate, T.Title, PRC.Guid AS ReportCardGuid
						  FROM PlayerReportCards PRC
						  INNER JOIN Players P ON PRC.PlayerId = P.PlayerId AND P.Guid = @PlayerGuid
						  INNER JOIN Squads S ON S.SquadId = P.SquadId
						  INNER JOIN Clubs C ON C.ClubId = S.ClubId AND C.Guid = @ClubGuid
						  INNER JOIN EvaluationTerms T ON PRC.TermId = T.TermId
						  WHERE T.Deleted IS NULL OR T.Deleted = 0
						ORDER BY T.StartDate DESC";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());
			p.Add("@PlayerGuid", playerId.ToString());

			using (var connection = factory.Connect()) {
				connection.Open();
				var reportCards = connection.Query(sql, p).Cast<IDictionary<string, object>>().Select<dynamic, MiniReportCard>(
						row => new MiniReportCard(Guid.Parse(row.ReportCardGuid.ToString())) {
							EndDate = row.EndDate,
							StartDate = row.StartDate,
							Title = row.Title
						}).ToList();

				return reportCards;
			}
		}
	}
}
