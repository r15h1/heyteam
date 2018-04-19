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

		public Assignment GetAssignment(Guid clubId, Guid assignmentId) {
			var sql = $@"SELECT	A.DueDate, 
		                        A.Guid AS AssignmentGuid, 
		                        CAST(A.CreatedOn AS DATE) AS CreatedOn, 
		                        A.Instructions,
		                        A.Title, Cl.Guid AS ClubGuid,                                
                                Co.FirstName + ' ' + Co.LastName AS CreatedBy,                                
                                (SELECT COUNT(1) FROM AssignmentTrainingMaterials ATM WHERE ATM.AssignmentId = A.AssignmentId) AS TrainingMaterialCount,
                                (SELECT COUNT(1) FROM PlayerAssignments PA WHERE PA.AssignmentId = A.AssignmentId) AS PlayerCount,
								STUFF ((SELECT DISTINCT COALESCE(S1.Name + ', ', '') FROM PlayerAssignments PA1 
								INNER JOIN Players P1 ON PA1.PlayerId = P1.PlayerId
								INNER JOIN Squads S1 ON P1.SquadId = S1.SquadId
								WHERE A.AssignmentId = PA1.AssignmentId 
								FOR XML PATH(''),TYPE ).value('.','VARCHAR(50)') 
									 ,1, 0, '') AS Squads
                        FROM Assignments A                                                 
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        INNER JOIN Coaches Co ON Cl.ClubId = Co.ClubId AND Co.CoachId = A.CoachId
						WHERE Cl.Guid = @ClubGuid AND A.Guid = @AssignmentGuid
						GROUP BY A.AssignmentId, A.DueDate, A.Guid, A.CreatedOn, A.Instructions, A.Title, Cl.Guid, Co.FirstName, Co.LastName;";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", clubId.ToString());
			p.Add("@AssignmentGuid", assignmentId.ToString());			
			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, dynamic>>();
				var assignment = reader.Select<dynamic, Assignment>(
						 row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString())) {
							 CreatedBy = row.CreatedBy,
							 CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
							 Instructions = row.Instructions,
							 Title = row.Title,
							 DueDate = row.DueDate.ToString("dd-MMM-yyyy"),
							 Players = row.PlayerCount,
							 TrainingMaterials = row.TrainingMaterialCount,
							 Squads = (row.Squads?.Trim().EndsWith(",") ? row.Squads.TrimEnd(new char[] { ',', ' ' }) : row.Squads)
						 }).SingleOrDefault();

				return assignment;
			}
		}
	

		public IEnumerable<Assignment> GetAssignments(AssignmentsRequest request) {
			var sql = $@"SELECT	A.DueDate, 
		                        A.Guid AS AssignmentGuid, 
		                        CAST(A.CreatedOn AS DATE) AS CreatedOn, 
		                        A.Instructions,
		                        A.Title, Cl.Guid AS ClubGuid,                                
                                Co.FirstName + ' ' + Co.LastName AS CreatedBy,                                
                                (SELECT COUNT(1) FROM AssignmentTrainingMaterials ATM WHERE ATM.AssignmentId = A.AssignmentId) AS TrainingMaterialCount,
                                COUNT(PA.AssignmentId) AS PlayerCount,
								STUFF ((SELECT DISTINCT COALESCE(S1.Name + ', ', '') FROM PlayerAssignments PA1 
								INNER JOIN Players P1 ON PA1.PlayerId = P1.PlayerId
								INNER JOIN Squads S1 ON P1.SquadId = S1.SquadId
								WHERE A.AssignmentId = PA1.AssignmentId 
								FOR XML PATH(''),TYPE ).value('.','VARCHAR(50)') 
									 ,1, 0, '') AS Squads
                        FROM Assignments A                                                 
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        INNER JOIN Coaches Co ON Cl.ClubId = Co.ClubId AND Co.CoachId = A.CoachId
                        LEFT JOIN PlayerAssignments PA ON A.AssignmentId = PA.AssignmentId  
                        LEFT JOIN Players P ON PA.PlayerId = P.PlayerId 
                        {((request.Squads?.Any() ?? false) ? " INNER JOIN Squads S ON S.ClubId = Cl.ClubId AND P.SquadId = S.SquadId " : "")}
                        WHERE Cl.Guid = @ClubGuid 
						{(request.Month.HasValue ? " AND MONTH(A.DueDate) = @Month " : "")}
						{(request.Year.HasValue ? " AND YEAR(A.DueDate) = @Year  " : "")}						
						{((request.Squads?.Any() ?? false) ? " AND S.Guid IN @Squads " : "")}
						{((request.Players?.Any() ?? false) ? " AND P.Guid IN @Players " : "")}

                        GROUP BY A.AssignmentId, A.DueDate, A.Guid, A.CreatedOn, A.Instructions, A.Title, Cl.Guid, Co.FirstName, Co.LastName;";

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
                            CreatedBy = row.CreatedBy,
                            CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
                            Instructions = row.Instructions,
                            Title = row.Title,
                            DueDate = row.DueDate.ToString("dd-MMM-yyyy"),
                            Players = row.PlayerCount,
                            TrainingMaterials = row.TrainingMaterialCount,
							Squads = (row.Squads?.Trim().EndsWith(",") ? row.Squads.TrimEnd(new char[] { ',', ' ' }) : row.Squads)
                        }).OrderBy(a => a.Title).ToList();
                   
                return assignments;
			}
		}

        public Assignment GetPlayerAssignment(PlayerAssignmentRequest request)
        {
            var sql = $@"SELECT	P.FirstName + ' ' + P.LastName + '(' + S.Name + ')' AS PlayerName,  
                                P.Guid AS PlayerGuid,		                                
                                PA.AssignedOn,
                                A.DueDate, 
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
                        WHERE P.Guid = @PlayerGuid AND A.Guid = @AssignmentGuid AND Cl.Guid = @ClubGuid;						
						
						SELECT T.Guid AS AssignmentGuid, T.Title, T.Description, T.ContentType, T.Url, T.ThumbnailUrl
						FROM AssignmentTrainingMaterials ATM
						INNER JOIN Assignments A ON A.AssignmentId = ATM.AssignmentId
						INNER JOIN TrainingMaterials T ON T.TrainingMaterialId = ATM.TrainingMaterialId
						WHERE A.Guid = @AssignmentGuid;";


            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", request.ClubId.ToString());
			p.Add("@AssignmentGuid", request.AssignmentId.ToString());
            p.Add("@PlayerGuid", request.PlayerId.ToString());
            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.QueryMultiple(sql, p);
				var assignmentCursor = reader.Read().Cast<IDictionary<string, dynamic>>();
				var assignment = assignmentCursor.Select<dynamic, Assignment>(
                        row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()))
                        {
                            CreatedBy = row.CreatedBy,
                            CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
                            Instructions = row.Instructions,
                            Title = row.Title,
                            DueDate = row.DueDate.ToString("dd-MMM-yyyy")
                        }).GroupBy(a => a.AssignmentId).Select(g => g.First()).SingleOrDefault();

    //            assignment.Allocations = assignmentCursor.Select<dynamic, PlayerAssignment>(
    //                       row => new PlayerAssignment(Guid.Parse(row.PlayerGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()))
    //                       {
    //                           AssignedBy = row.AssignedBy,
    //                           AssignedOn = row.AssignedOn.ToString("dd-MMM-yyyy"),
    //                           PlayerName = row.PlayerName
    //                       }
    //                    ).ToList();

				//var trainingMaterialCursor = reader.Read().Cast<IDictionary<string, dynamic>>();
				//assignment.TrainingMaterials = trainingMaterialCursor.Select<dynamic, MiniTrainingMaterial>( 
				//	row => new MiniTrainingMaterial(Guid.Parse(row.AssignmentGuid.ToString()), row.Title){
				//		Description = row.Description, ThumbnailUrl = row.ThumbnailUrl, Url = row.Url
				//	}				
				//).ToList();

				return assignment;
            }
        }		
	}
}
