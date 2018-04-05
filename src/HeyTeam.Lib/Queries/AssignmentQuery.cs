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

        public IEnumerable<Assignment> GetAssignments(Guid clubId)
        {
            var sql = @"SELECT	P.FirstName + ' ' + P.LastName AS PlayerName, 
                                P.Guid AS PlayerGuid,
		                        PA.Guid AS PlayerAssignmnentGuid,
                                PA.DateDue, 
		                        A.Guid AS AssignmentGuid, 
		                        CAST(A.CreatedOn AS DATE) AS CreatedOn, 
		                        A.Instructions,
		                        A.Title, Cl.Guid AS ClubGuid,
                                Co.Guid AS CoachGuid,
		                        Co.FirstName + ' ' + Co.LastName AS CoachName
                        FROM PlayerAssignments PA
                        INNER JOIN Assignments A ON A.AssignmentId = PA.AssignmentId
                        INNER JOIN Players P ON PA.PlayerId = P.PlayerId
                        INNER JOIN Clubs Cl ON A.ClubId = Cl.ClubId
                        INNER JOIN Squads S ON S.ClubId = Cl.ClubId AND P.SquadId = S.SquadId
                        INNER JOIN Coaches Co ON Cl.ClubId = Co.ClubId AND Co.CoachId = A.CoachId                        
                        WHERE Cl.Guid = @ClubGuid
                        ORDER BY CreatedOn DESC, DateDue DESC, PlayerName;";
            DynamicParameters p = new DynamicParameters();
            p.Add("@ClubGuid", clubId.ToString());

            using (var connection = connectionFactory.Connect())
            {
                connection.Open();
                var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
                var list = reader.Select<dynamic, Assignment>(
                        row => new Assignment(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.AssignmentGuid.ToString()), Guid.Parse(row.PlayerAssignmnentGuid.ToString()))
                        {
                            Coach = new MiniModel(Guid.Parse(row.CoachGuid.ToString()), row.CoachName),                            
                            CreatedOn = row.CreatedOn,
                            DateDue = row.DateDue,
                            Instructions = row.Instructions,
                            Player = new MiniModel(Guid.Parse(row.PlayerGuid.ToString()), row.PlayerName),
                            Title = row.Title
                        }).ToList();

                return list;
            }
        }
    }
}
