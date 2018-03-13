using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Repositories {
	public class EvaluationRepository : IEvaluationRepository {
		private readonly IDbConnectionFactory connectionFactory;
		public EvaluationRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddTerm(TermSetupRequest request) {
			ThrowIf.ArgumentIsNull(request);
			string sql = @"INSERT INTO EvaluationTerms (Guid, ClubId, Title, TermStatusId, StartDate, EndDate)
							VALUES(	
									@TermGuid, 
									(SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid),
									@Title, 1, @StartDate, @EndDate							
							)";

			
			DynamicParameters p = new DynamicParameters();
			p.Add("@TermGuid", Guid.NewGuid().ToString());
            p.Add("@ClubGuid", request.ClubId.ToString());
            p.Add("@Title", request.Title);
            p.Add("@StartDate", request.StartDate);
            p.Add("@EndDate", request.EndDate);

            using (var connection = connectionFactory.Connect()) {
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void DeleteTerm(TermDeleteRequest request) {
			throw new System.NotImplementedException();
		}

		public Guid GeneratePlayerReportCard(GenerateReportCardRequest request) {
			ThrowIf.ArgumentIsNull(request);
			string sql = @"INSERT INTO PlayerReportCards (
								TermId, ReportCardDesignId, PlayerId,
								Guid, CreatedOn, lastModifiedOn)
							VALUES(	
								(SELECT TermId FROM EvaluationTerms WHERE Guid = @TermGuid),
								(SELECT ReportCardDesignId FROM ReportCardDesigns WHERE Guid = @ReportCardDesignGuid),
								(SELECT PlayerId FROM Players WHERE Guid = @PlayerGuid),
								@ReportCardGuid, GetDate(), GetDate()
							);";

			var reportCardGuid = Guid.NewGuid();
			DynamicParameters p = new DynamicParameters();
			p.Add("@TermGuid", request.TermId.ToString());			
			p.Add("@ReportCardDesignGuid", request.ReportDesignId.ToString());
			p.Add("@ReportCardGuid", reportCardGuid.ToString());
			p.Add("@PlayerGuid", request.PlayerId.ToString());

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				connection.ExecuteScalar(sql, p);
			}

			return reportCardGuid;
		}

        public void UpdatePlayerReportCard(UpdateReportCardRequest request)
        {
            ThrowIf.ArgumentIsNull(request);
            string sql = $@"DELETE PlayerReportCardGrades 
                            WHERE PlayerReportCardId = (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId)
                                AND ReportCardSkillId = (SELECT ReportCardSkillId FROM ReportCardSkills WHERE Guid = @ReportCardSkillId);
                            {(request.ReportCardGrade.HasValue?
                                @"INSERT INTO PlayerReportCardGrades (PlayerReportCardId, ReportCardSkillId, ReportCardGradeId)
							    VALUES( (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId), 
                                        (SELECT ReportCardSkillId FROM ReportCardSkills WHERE Guid = @ReportCardSkillId), 
                                        @ReportCardGradeId)"
                                : "")
                            };";

            var reportCardGuid = Guid.NewGuid();
            DynamicParameters p = new DynamicParameters();            
            p.Add("@PlayerReportCardId", request.ReportCardId.ToString());
            p.Add("@ReportCardSkillId", request.SkillId.ToString());
            p.Add("@ReportCardGradeId", (request.ReportCardGrade.HasValue ? (int?)request.ReportCardGrade : null));

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {                        
                        connection.Execute(sql, p, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void UpdateTerm(TermSetupRequest request) {
			throw new System.NotImplementedException();
		}
	}
}
