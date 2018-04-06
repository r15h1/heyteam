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
                                Co1.FirstName + ' ' + Co1.LastName AS CreatedBy
                        FROM PlayerAssignments PA
                        INNER JOIN Assignments A ON A.AssignmentId = PA.AssignmentId
                        INNER JOIN Players P ON PA.PlayerId = P.PlayerId
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        INNER JOIN Squads S ON S.ClubId = Cl.ClubId AND P.SquadId = S.SquadId
                        INNER JOIN Coaches Co1 ON Cl.ClubId = Co1.ClubId AND Co1.CoachId = PA.CoachId
                        INNER JOIN Coaches Co2 ON Cl.ClubId = Co2.ClubId AND Co2.CoachId = A.CoachId
                        WHERE Cl.Guid = @ClubGuid AND YEAR(PA.AssignedOn) = @Year AND MONTH(PA.AssignedOn) = @Month
						{((request.Squads?.Any() ?? false) ? " AND S.Guid IN @Squads " : "")}
                        ORDER BY CreatedOn DESC, DateDue DESC, PlayerName;";

			DynamicParameters p = new DynamicParameters();
			p.Add("@ClubGuid", request.ClubId.ToString());
			p.Add("@Month", request.Month);
			p.Add("@Year", request.Year);			

			if (request.Squads?.Any() ?? false)
				p.Add("@Squads", request.Squads);

			using (var connection = connectionFactory.Connect()) {
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, dynamic>>();
                var assignments = reader.Select<dynamic, Assignment>(
                        row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()), Guid.Parse(row.PlayerAssignmnentGuid.ToString()))
                        {
                            Createdby = row.CreatedBy,
                            CreatedOn = row.CreatedOn.ToString("dd-MMM-yyyy"),
                            Instructions = row.Instructions,
                            Title = row.Title
                        }).GroupBy(a => a.AssignmentId).Select(g => g.First()).ToList();

                var allocations = reader.Select<dynamic, PlayerAssignment>(
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
	}
}
