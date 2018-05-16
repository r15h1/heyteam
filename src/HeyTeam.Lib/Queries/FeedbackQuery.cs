using Dapper;
using HeyTeam.Core.Models.Mini;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeyTeam.Lib.Queries
{
    public class FeedbackQuery : IFeedbackQuery
    {
        private IDbConnectionFactory connectionFactory;

        public FeedbackQuery(IDbConnectionFactory factory)
        {
            ThrowIf.ArgumentIsNull(factory);
            this.connectionFactory = factory;
        }

        public IEnumerable<MiniFeedback> GetFeedbackList(FeedbackListRequest request)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", request.ClubId.ToString());
            p.Add("@SquadGuid", request.SquadId.ToString());
            p.Add("@Year", request.Year);
            p.Add("@Week", request.Week);

            var sql = @"SELECT F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote <br/>' +  Comments FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
	                        STUFF ((SELECT COALESCE(EA.Feedback + '<br/> ', '') 
			                        FROM EventAttendance EA 
			                        INNER JOIN Players P1 ON EA.PlayerId = P1.PlayerId
			                        INNER JOIN Events E ON EA.EventId = E.EventId
			                        WHERE YEAR(E.StartDate) = @Year AND DATEPART(wk, E.StartDate) = @Week AND P1.PlayerId = P.PlayerId
			                        FOR XML PATH(''),TYPE ).value('.','VARCHAR(2000)') 
					                        ,1, 0, '') AS WeeklyNotes
                        FROM Players P
                        INNER JOIN Squads S ON S.SquadId = P.SquadId
                        LEFT JOIN Feedback F ON P.PlayerId = F.PlayerId AND F.Year = @Year AND F.Week = @Week
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) AND S.Guid = @SquadGuid;";

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var feedback = reader.Select<dynamic, MiniFeedback>(
                        row => new MiniFeedback((row.FeedbackGuid == null ? Guid.Empty : Guid.Parse(row.FeedbackGuid?.ToString())))
                        {
                            LatestComment = row.LatestComment,
                            Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), row.PlayerName),
                            PublishedOn = row.PublishedOn,
                            WeeklyNotes = row.WeeklyNotes
                        }).ToList();

                return feedback;
            }
        }

        public MiniFeedbackChain GetFeedbackChain(FeedbackChainRequest request)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", request.ClubId.ToString());
            p.Add("@FeedbackGuid", request.FeedbackId.ToString());

            var sql = @"SELECT F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote <br/>' +  Comments FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
	                        '' AS WeeklyNotes,
                            F.Year, F.Week
                        FROM Players P                        
                        INNER JOIN Feedback F ON P.PlayerId = F.PlayerId 
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) AND F.Guid = @FeedbackGuid;
            
                        SELECT CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote <br/>' +  Comments AS Comments
                        FROM FeedbackComments FC 
                        INNER JOIN Feedback F ON F.FeedbackId = FC.FeedbackId
                        WHERE F.Guid = @FeedbackGuid
                        ORDER BY CreatedOn;
                    ";

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.QueryMultiple(sql, p);
                var feedback = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, MiniFeedbackChain>(
                        row => new MiniFeedbackChain{
                            Feedback = new MiniFeedback((row.FeedbackGuid == null ? Guid.Empty : Guid.Parse(row.FeedbackGuid?.ToString())))
                            {
                                LatestComment = row.LatestComment,
                                Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), row.PlayerName),
                                PublishedOn = row.PublishedOn,
                                WeeklyNotes = row.WeeklyNotes
                            },
                            Year = row.Year,
                            Week = row.Week
                        }).SingleOrDefault();

                feedback.Comments = reader.Read().Cast<IDictionary<string, object>>().Select<dynamic, string>(
                    row => row.Comments
                ).ToList();

                return feedback;
            }
        }


    }
}