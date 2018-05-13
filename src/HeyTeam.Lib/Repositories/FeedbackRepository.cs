using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Lib.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public FeedbackRepository(IDbConnectionFactory factory)
        {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }
        public Response PublishFeedback(FeedbackPublishRequest request)
        {
            var sql = @"DECLARE @FeedbackId BIGINT;
                        
                        INSERT INTO Feedback(Year, Week, SquadId, PlayerId, SquadName, Guid, Published, PublishedOn, CoachId)
                        VALUES(@Year, @Week,
                            (SELECT SquadId FROM Players WHERE Guid = @PlayerGuid),
                            (SELECT PlayerId FROM Players WHERE Guid = @PlayerGuid),
                            (SELECT Name FROM Players P INNER JOIN Squads S ON S.SquadId = P.SquadId WHERE P.Guid = @PlayerGuid),
                            NewId(),
                            1,
                            GetDate(),
                            (SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid)
                        );

                        SELECT @FeedbackId = SCOPE_IDENTITY();
                        
                        INSERT INTO FeedbackComments (FeedbackId, CreatedOn, PostedBy, PosterId, Comments)
                        VALUES( @FeedbackId, GetDate(), 
                            (SELECT FirstName + ' ' + LastName FROM Coaches WHERE Guid = @CoachGuid),
                            (SELECT CoachId FROM Coaches WHERE Guid = @CoachGuid),
                            @Comments
                        );
                    ";

            var parameters = new DynamicParameters();
            parameters.Add("@Year", request.Year);
            parameters.Add("@Week", request.Week);
            parameters.Add("@PlayerGuid", request.PlayerId);
            parameters.Add("@CoachGuid", request.CoachId);
            parameters.Add("@Comments", request.Comments);

            using (var connection = connectionFactory.Connect()) {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) {
                    try
                    {
                        connection.Execute(sql, parameters, transaction);
                        transaction.Commit();
                        return Response.CreateSuccessResponse();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        return Response.CreateResponse(ex);
                    }
                }
            }            
        }
    }
}
