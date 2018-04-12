using Dapper;
using HeyTeam.Core.Models;
using HeyTeam.Core.Models.Mini;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries
{
    public class AssignmentQuery : IAssignmentQuery
    {
        private readonly IDbConnectionFactory connectionFactory;

        public AssignmentQuery(IDbConnectionFactory factory)
        {        
            this.connectionFactory = factory;
        }

		public IEnumerable<Assignment> GetAssignments(AssignmentsRequest request) {
			var sql = $@"SELECT	P.FirstName + ' ' + P.LastName + '(' + S.Name + ')' AS PlayerName,  
                                P.Guid AS PlayerGuid,
		                        PA.Guid AS PlayerAssignmnentGuid,
                                PA.DateDue, 
                                PA.AssignedOn,
		                        A.Guid AS AssignmentGuid, 
		                        CAST(A.CreatedOn AS DATE) AS CreatedOn, 
		                        A.Instructions,
		                        A.Title, Cl.Guid AS ClubGuid,
                                Co1.FirstName + ' ' + Co1.LastName AS AssignedBy,
                                Co2.FirstName + ' ' + Co2.LastName AS CreatedBy
                        FROM Assignments A 
                        LEFT JOIN PlayerAssignments PA ON A.AssignmentId = PA.AssignmentId
                        LEFT JOIN Players P ON PA.PlayerId = P.PlayerId
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        LEFT JOIN Squads S ON S.ClubId = Cl.ClubId AND P.SquadId = S.SquadId
                        LEFT JOIN Coaches Co1 ON Cl.ClubId = Co1.ClubId AND Co1.CoachId = PA.CoachId
                        LEFT JOIN Coaches Co2 ON Cl.ClubId = Co2.ClubId AND Co2.CoachId = A.CoachId
                        WHERE Cl.Guid = @ClubGuid 
						{(request.Month.HasValue ? " AND MONTH(PA.AssignedOn) = @Month " : "")}
						{(request.Year.HasValue ? " AND YEAR(PA.AssignedOn) = @Year  " : "")}						
						{((request.Squads?.Any() ?? false) ? " AND S.Guid IN @Squads " : "")}
						{((request.Players?.Any() ?? false) ? " AND P.Guid IN @Players " : "")}";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", request.ClubId.ToString());

			if(request.Month.HasValue)
				p.Add("@Month", request.Month);

			if (request.Year.HasValue)
				p.Add("@Year", request.Year);			

			if (request.Squads?.Any() ?? false)
				p.Add("@Squads", request.Squads);

			if (request.Players?.Any() ?? false)
				p.Add("@Players", request.Players);

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p);
                var assignments = reader.Cast<IDictionary<string, dynamic>>().Select<dynamic, Assignment>(
                        row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()))
                        {
                            Createdby = row.CreatedBy,
                            CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
                            Instructions = row.Instructions,
                            Title = row.Title
                        }).GroupBy(a => a.AssignmentId).Select(g => g.First()).OrderBy(a => a.Title).ToList();

                var allocations = reader.Where(x => x.PlayerAssignmnentGuid != null).Select<dynamic, PlayerAssignment>(
                           row => new PlayerAssignment(Guid.Parse(row.PlayerGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()), Guid.Parse(row.PlayerAssignmnentGuid.ToString()))
                           {
                               AssignedBy = row.AssignedBy,
                               AssignedOn = row.AssignedOn.ToString("dd-MMM-yyyy"),
                               DueDate = row.DateDue?.ToString("dd-MMM-yyyy"),
                               PlayerName = row.PlayerName
                           }
                        ).ToList();

                foreach (var assignment in assignments)
                    assignment.Allocations = allocations.Where(a => a.AssignmentId == assignment.AssignmentId).ToList();
                   
                return assignments;
			}
		}

        public Assignment GetPlayerAssignment(PlayerAssignmentRequest request)
        {
            var sql = $@"SELECT	P.FirstName + ' ' + P.LastName + '(' + S.Name + ')' AS PlayerName,  
                                P.Guid AS PlayerGuid,
		                        PA.Guid AS PlayerAssignmnentGuid,
                                PA.DateDue, 
                                PA.AssignedOn,
		                        A.Guid AS AssignmentGuid, 
		                        CAST(A.CreatedOn AS DATE) AS CreatedOn, 
		                        A.Instructions,
		                        A.Title, Cl.Guid AS ClubGuid,
                                Co1.FirstName + ' ' + Co1.LastName AS AssignedBy,
                                Co1.FirstName + ' ' + Co1.LastName AS CreatedBy
                        FROM PlayerAssignments PA
                        INNER JOIN Assignments A ON A.AssignmentId = PA.AssignmentId
                        INNER JOIN Players P ON PA.PlayerId = P.PlayerId
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        INNER JOIN Squads S ON S.ClubId = Cl.ClubId AND P.SquadId = S.SquadId
                        INNER JOIN Coaches Co1 ON Cl.ClubId = Co1.ClubId AND Co1.CoachId = PA.CoachId
                        INNER JOIN Coaches Co2 ON Cl.ClubId = Co2.ClubId AND Co2.CoachId = A.CoachId
                        WHERE A.Guid = @AssignmentGuid AND Cl.Guid = @ClubGuid AND PA.Guid = @PlayerAssignmentGuid;						
						
						SELECT T.Guid AS AssignmentGuid, T.Title, T.Description, T.ContentType, T.Url, T.ThumbnailUrl
						FROM AssignmentTrainingMaterials ATM
						INNER JOIN Assignments A ON A.AssignmentId = ATM.AssignmentId
						INNER JOIN TrainingMaterials T ON T.TrainingMaterialId = ATM.TrainingMaterialId
						WHERE A.Guid = @AssignmentGuid;";


            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", request.ClubId.ToString());
			p.Add("@AssignmentGuid", request.AssignmentId.ToString());
			p.Add("@PlayerAssignmentGuid", request.PlayerAssignmentId.ToString());
			using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.QueryMultiple(sql, p);
				var assignmentCursor = reader.Read().Cast<IDictionary<string, dynamic>>();
				var assignment = assignmentCursor.Select<dynamic, Assignment>(
                        row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()))
                        {
                            Createdby = row.CreatedBy,
                            CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
                            Instructions = row.Instructions,
                            Title = row.Title
                        }).GroupBy(a => a.AssignmentId).Select(g => g.First()).SingleOrDefault();

                assignment.Allocations = assignmentCursor.Select<dynamic, PlayerAssignment>(
                           row => new PlayerAssignment(Guid.Parse(row.PlayerGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()), Guid.Parse(row.PlayerAssignmnentGuid.ToString()))
                           {
                               AssignedBy = row.AssignedBy,
                               AssignedOn = row.AssignedOn.ToString("dd-MMM-yyyy"),
                               DueDate = row.DateDue?.ToString("dd-MMM-yyyy"),
                               PlayerName = row.PlayerName
                           }
                        ).ToList();

				var trainingMaterialCursor = reader.Read().Cast<IDictionary<string, dynamic>>();
				assignment.TrainingMaterials = trainingMaterialCursor.Select<dynamic, MiniTrainingMaterial>( 
					row => new MiniTrainingMaterial(Guid.Parse(row.AssignmentGuid.ToString()), row.Title){
						Description = row.Description, ThumbnailUrl = row.ThumbnailUrl, Url = row.Url
					}				
				).ToList();

				return assignment;
            }
        }		
	}
}
