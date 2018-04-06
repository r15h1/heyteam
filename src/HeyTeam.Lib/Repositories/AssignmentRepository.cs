using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HeyTeam.Lib.Repositories {
	public class AssignmentRepository : IAssignmentRepository {
		private readonly IDbConnectionFactory factory;

		public AssignmentRepository(IDbConnectionFactory factory){
			this.factory = factory;
		}

		public void CreateAssignment(AssignmentRequest request)
        {
            using (var connection = factory.Connect())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var assignmentGuid = Guid.NewGuid();
                        CreateAssignment(assignmentGuid, request, connection, transaction);
                        AddTrainingMaterials(assignmentGuid, request.TrainingMaterials, connection, transaction);
                        AssignToSquads(assignmentGuid, request, connection, transaction);
                        AssignToPlayers(assignmentGuid, request, connection, transaction);
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

        private static void CreateAssignment(Guid guid, AssignmentRequest request, IDbConnection connection, IDbTransaction transaction)
        {
            var sql = @"INSERT INTO Assignments(Guid, ClubId, CreatedOn, CoachId, Title, Instructions)
                        SELECT @AssignmentGuid, Cl.ClubId, GetDate(), Co.CoachId, @Title, @Instructions
                        FROM Clubs Cl
                        INNER JOIN Coaches Co ON Co.ClubId = Cl.ClubId
                        WHERE Co.Guid = @CoachGuid AND Cl.Guid = @ClubGuid;";
            
            var parameters = new DynamicParameters();
            parameters.Add("@AssignmentGuid", guid);
            parameters.Add("@ClubGuid", request.ClubId);
            parameters.Add("@CoachGuid", request.CoachId);
            parameters.Add("@Title", request.Title);
            parameters.Add("@Instructions", request.Instructions);
            connection.Execute(sql, parameters, transaction);
        }

        private void AddTrainingMaterials(Guid asssignmentId, IEnumerable<Guid> trainingMaterials, IDbConnection connection, IDbTransaction transaction)
        {
            if(trainingMaterials?.Any() ?? false)
            {
                var sql = @"INSERT INTO AssignmentTrainingMaterials (AssignmentId, TrainingMaterialId)
                            SELECT
                                (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                                TrainingMaterialId 
                            FROM TrainingMaterials WHERE Guid IN @TrainingMaterials;";
                
                var parameters = new DynamicParameters();
                parameters.Add("@AssignmentGuid", asssignmentId);
                parameters.Add("@TrainingMaterials", trainingMaterials);
                connection.Execute(sql, parameters, transaction);                
            }
        }

        private void AssignToSquads(Guid assignmentGuid, AssignmentRequest request, IDbConnection connection, IDbTransaction transaction)
        {
            if(request.Squads?.Any() ?? false)
            {
                var sql = @"INSERT INTO PlayerAssignments (PlayerId, AssignmentId, AssignedOn, CoachId, DateDue)
                            SELECT P.PlayerId,
                                (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                                GetDate(),
                                (Select CoachId FROM Coaches WHERE Guid = @CoachGuid),
                                @DateDue
                            FROM Players P
                            INNER JOIN Squads S ON P.SquadId = S.SquadId
                            WHERE S.Guid IN @Squads;";
                var parameters = new DynamicParameters();
                parameters.Add("@AssignmentGuid", assignmentGuid);
                parameters.Add("@Squads", request.Squads);
                parameters.Add("@CoachGuid", request.CoachId);
                parameters.Add("@DateDue", request.DateDue);
                connection.Execute(sql, parameters, transaction);
            }
        }

        private void AssignToPlayers(Guid assignmentGuid, AssignmentRequest request, IDbConnection connection, IDbTransaction transaction)
        {
            if (request.Players?.Any() ?? false)
            {
                var sql = @"INSERT INTO PlayerAssignments (PlayerId, AssignmentId, AssignedOn, CoachId, DateDue)
                            SELECT P.PlayerId,
                                (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                                GetDate(),
                                (Select CoachId FROM Coaches WHERE Guid = @CoachGuid),
                                @DateDue
                            FROM Players P
                            WHERE P.Guid IN @Players;";
                var parameters = new DynamicParameters();
                parameters.Add("@AssignmentGuid", assignmentGuid);
                parameters.Add("@Players", request.Players);
                parameters.Add("@CoachGuid", request.CoachId);
                parameters.Add("@DateDue", request.DateDue);
                connection.Execute(sql, parameters, transaction);
            }
        }

        public void DeletePlayerAssignment(Guid playerAssignmentId)
        {
            var sql = @"DELETE FROM PlayerAssignments WHERE Guid = @PlayerAssignmentId;";
            var parameters = new DynamicParameters();
            parameters.Add("@PlayerAssignmentId", playerAssignmentId);
            using (var connection = factory.Connect())
            {
                connection.Open();
                connection.Execute(sql, parameters);
            }
        }
    }
}
