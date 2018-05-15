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
								(SELECT PlayerId FROM Players P WHERE Guid = @PlayerGuid AND (P.Deleted IS NULL OR P.Deleted = 0)),
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

        public void UpdatePlayerReportCard(UpdateReportCardRequest request) {
			ThrowIf.ArgumentIsNull(request);						
			var command = GetReportCardUpdateCommand(request);

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(command.Sql, command.Parameters, transaction);
						transaction.Commit();
					} catch (Exception ex) {
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}

		private (string Sql, DynamicParameters Parameters) GetReportCardUpdateCommand(UpdateReportCardRequest request) {
			string sql = null;
			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("@PlayerReportCardId", request.ReportCardId.ToString());

			if (request.SkillId.HasValue) {
				sql = $@"DELETE PlayerReportCardGrades 
                            WHERE PlayerReportCardId = (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId)
                                AND ReportCardSkillId = (SELECT ReportCardSkillId FROM ReportCardSkills WHERE Guid = @ReportCardSkillId);
                            {(request.ReportCardGrade.HasValue ?
								@"INSERT INTO PlayerReportCardGrades (PlayerReportCardId, ReportCardSkillId, ReportCardGradeId)
							    VALUES( (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId), 
                                        (SELECT ReportCardSkillId FROM ReportCardSkills WHERE Guid = @ReportCardSkillId), 
                                        @ReportCardGradeId)"
												: "")
											};";							
				
				parameters.Add("@ReportCardSkillId", request.SkillId.ToString());
				parameters.Add("@ReportCardGradeId", (request.ReportCardGrade.HasValue ? (int?)request.ReportCardGrade : null));
			} else if(request.Facet != null && !request.Facet.Key.IsEmpty()){
				sql = $@"DELETE PlayerReportCardFacets 
                            WHERE PlayerReportCardId = (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId)
                                AND FacetKey = @FacetKey;
                            {(!request.Facet.Value.IsEmpty() ?
								@"INSERT INTO PlayerReportCardFacets (PlayerReportCardId, FacetKey, FacetValue)
							    VALUES( (SELECT PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardId), 
                                        @FacetKey, @FacetValue)"
												: "")
											};";

				parameters.Add("@FacetKey", request.Facet.Key);
				parameters.Add("@FacetValue", request.Facet?.Value);
			}
			return (sql, parameters);
		}

		public void UpdateTerm(TermSetupRequest request) {
			throw new System.NotImplementedException();
		}

        public void DeletePlayerReportCard(DeleteReportCardRequest request)
        {
            var sql = @"DECLARE @PlayerReportCardId BIGINT, @PlayerId BIGINT;
                        
                        SELECT @PlayerId = PlayerId 
                        FROM Players P
                        INNER JOIN Squads S ON S.SquadId = P.SquadId
                        INNER JOIN Clubs C ON S.ClubId = C.ClubId
                        WHERE P.Guid = @PlayerGuid AND C.Guid = @ClubGuid AND S.Guid = @SquadGuid;
                        
                        SELECT @PlayerReportCardId = PlayerReportCardId FROM PlayerReportCards WHERE Guid = @PlayerReportCardGuid AND PlayerId = @PlayerId;

                        DELETE PlayerReportCardFacets WHERE PlayerReportCardId = @PlayerReportCardId;
                        DELETE PlayerReportCardGrades WHERE PlayerReportCardId = @PlayerReportCardId;
                        DELETE PlayerReportCards WHERE PlayerReportCardId = @PlayerReportCardId;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ClubGuid", request.ClubId.ToString());
            parameters.Add("@SquadGuid", request.SquadId.ToString());
            parameters.Add("@PlayerGuid", request.PlayerId.ToString());
            parameters.Add("@PlayerReportCardGuid", request.ReportCardId.ToString());

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(sql, parameters, transaction);
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
    }
}
