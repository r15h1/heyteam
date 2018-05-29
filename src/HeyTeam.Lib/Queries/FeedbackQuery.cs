using Dapper;
using HeyTeam.Core;
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

        public IEnumerable<MiniFeedback> GetFeedbackList(SquadFeedbackListRequest request)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", request.ClubId.ToString());
            p.Add("@SquadGuid", request.SquadId.ToString());
            p.Add("@Year", request.Year);
            p.Add("@Week", request.Week);

            var sql = @"SELECT F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
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
	                        (SELECT TOP 1 '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
	                        '' AS WeeklyNotes,
                            F.Year, F.Week
                        FROM Players P                        
                        INNER JOIN Feedback F ON P.PlayerId = F.PlayerId 
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) AND F.Guid = @FeedbackGuid;
            
                        SELECT '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments AS Comments
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

		public IEnumerable<MiniFeedback> GetFeedbackList(PlayerFeedbackListRequest request) {
			var weekRange = GetWeekRange(request.Year, request.Month);

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", request.ClubId.ToString());
			p.Add("@PlayerGuid", request.PlayerId.ToString());
			p.Add("@Year", request.Year);
			p.Add("@MinWeek", weekRange.MinWeek);
			p.Add("@MaxWeek", weekRange.MaxWeek);

			var sql = @"SELECT F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments 
								FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment	                    
                        FROM Players P
                        INNER JOIN Squads S ON S.SquadId = P.SquadId
						INNER JOIN Clubs C ON C.ClubId = S.ClubId
                        INNER JOIN Feedback F ON P.PlayerId = F.PlayerId AND F.Year = @Year AND F.Week BETWEEN @MinWeek AND @MaxWeek
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) AND P.Guid = @PlayerGuid AND C.Guid = @ClubGuid;";

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var feedback = reader.Select<dynamic, MiniFeedback>(
						row => new MiniFeedback((row.FeedbackGuid == null ? Guid.Empty : Guid.Parse(row.FeedbackGuid?.ToString()))) {
							LatestComment = row.LatestComment,
							Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), row.PlayerName),
							PublishedOn = row.PublishedOn
						}).ToList();

				return feedback;
			}
		}

		private (int MinWeek, int MaxWeek) GetWeekRange(int year, int month) {
			DateTime min = new DateTime(year, month, 1), max = new DateTime(year, month, DateTime.DaysInMonth(year, month));
			return (min.GetWeekOfYear(), max.GetWeekOfYear());		
		}

		public IEnumerable<MiniFeedback> GetLatestFeedback(LatestFeedbackRequest request) {
			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", request.ClubId.ToString());
			p.Add("@MemberGuid", request.MemberId.ToString());
			string sql = GetLatestFeedbackSql(request.Membership);

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var feedback = reader.Select<dynamic, MiniFeedback>(
						row => new MiniFeedback((row.FeedbackGuid == null ? Guid.Empty : Guid.Parse(row.FeedbackGuid?.ToString()))) {
							LatestComment = row.LatestComment,
							Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), row.PlayerName),
							PublishedOn = row.PublishedOn,
							WeeklyNotes = row.WeeklyNotes
						}).ToList();

				return feedback;
			}
		}

		private static string GetLatestFeedbackSql(Membership membership) {
			if (membership == Membership.Coach) {
				return @"DECLARE @Squads TABLE(SquadId BIGINT);
                        INSERT INTO @Squads(SquadId) 
                        SELECT S.SquadId 
                        FROM SquadCoaches S 
                        INNER JOIN Coaches CO ON CO.CoachId = S.CoachId 
                        WHERE (CO.Deleted IS NULL OR CO.Deleted = 0) AND CO.Guid = @MemberGuid;		
			
                        SELECT TOP 10 F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments 
		                        FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
		                        (SELECT TOP 1 CreatedOn 
		                        FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS CreatedOn
	
                        FROM Players P
                        INNER JOIN @Squads S ON S.SquadId = P.SquadId
                        INNER JOIN Feedback F ON P.PlayerId = F.PlayerId 
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) 
                        ORDER BY CreatedOn DESC, F.PublishedOn DESC;";
			} else {
				return @"SELECT TOP 10 F.Guid AS FeedbackGuid, P.Guid AS PlayerGuid, P.FirstName + ' ' + P.LastName AS PlayerName, F.PublishedOn,
	                        (SELECT TOP 1 '<strong>' + CAST(CreatedOn AS VARCHAR(20)) + ': ' + PostedBy + ' wrote</strong><br/>' +  Comments 
		                        FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS LatestComment,
		                        (SELECT TOP 1 CreatedOn 
		                        FROM FeedbackComments FC WHERE FC.FeedbackId = F.FeedbackId ORDER BY CreatedOn DESC) AS CreatedOn
	
                        FROM Players P
                        INNER JOIN Squads S ON S.SquadId = P.SquadId
                        INNER JOIN Feedback F ON P.PlayerId = F.PlayerId 
                        WHERE (P.Deleted IS NULL OR P.Deleted = 0) AND P.Guid = @MemberGuid
                        ORDER BY CreatedOn DESC, F.PublishedOn DESC;";
			}
		}
	}
}