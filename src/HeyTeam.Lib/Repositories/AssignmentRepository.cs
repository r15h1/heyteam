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

						if(request.AssignedTo==AssignedTo.SelectedSquads)
							AssignToSquads(assignmentGuid, request, connection, transaction);
						else
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
            var sql = @"INSERT INTO Assignments(Guid, ClubId, CreatedOn, CoachId, Title, Instructions, DueDate)
                        SELECT @AssignmentGuid, Cl.ClubId, GetDate(), Co.CoachId, @Title, @Instructions, @DueDate
                        FROM Clubs Cl
                        INNER JOIN Coaches Co ON Co.ClubId = Cl.ClubId
                        WHERE Co.Guid = @CoachGuid AND Cl.Guid = @ClubGuid;";
            
            var parameters = new DynamicParameters();
            parameters.Add("@AssignmentGuid", guid);
            parameters.Add("@ClubGuid", request.ClubId);
            parameters.Add("@CoachGuid", request.CoachId);
            parameters.Add("@Title", request.Title);
            parameters.Add("@Instructions", request.Instructions);
            parameters.Add("@DueDate", request.DueDate);
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
            var sql = @"INSERT INTO PlayerAssignments (PlayerId, AssignmentId, AssignedOn, CoachId)
                        SELECT P.PlayerId,
                            (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                            GetDate(),
                            (Select CoachId FROM Coaches WHERE Guid = @CoachGuid)
                        FROM Players P
                        INNER JOIN Squads S ON P.SquadId = S.SquadId
                        WHERE S.Guid IN @Squads;";
            var parameters = new DynamicParameters();
            parameters.Add("@AssignmentGuid", assignmentGuid);
            parameters.Add("@Squads", request.Squads);
            parameters.Add("@CoachGuid", request.CoachId);
            
            connection.Execute(sql, parameters, transaction);            
        }

        private void AssignToPlayers(Guid assignmentGuid, AssignmentRequest request, IDbConnection connection, IDbTransaction transaction)
        {            
            var sql = $@"INSERT INTO PlayerAssignments (PlayerId, AssignmentId, AssignedOn, CoachId)
                        SELECT P.PlayerId,
                            (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                            GetDate(),
                            (Select CoachId FROM Coaches WHERE Guid = @CoachGuid)
                        FROM Players P
                        {(request.AssignedTo == AssignedTo.IndividualPlayers ? 
							"WHERE P.Guid IN @Players" 
							: $@"INNER JOIN Squads S ON P.SquadId = S.SquadId 
								 INNER JOIN Clubs C ON S.ClubId = C.ClubId AND C.Guid = @ClubGuid"			
						)};";
            var parameters = new DynamicParameters();
            parameters.Add("@AssignmentGuid", assignmentGuid);
            parameters.Add("@Players", request.Players);
            parameters.Add("@CoachGuid", request.CoachId);
			parameters.Add("@ClubGuid", request.ClubId.ToString());
			connection.Execute(sql, parameters, transaction);            
        }

        public void DeletePlayerAssignment(PlayerAssignmentRequest request)
        {
            var sql = $@"DELETE PA FROM PlayerAssignments PA
                            INNER JOIN Players P ON PA.PlayerId = P.PlayerId
                            INNER JOIN Assignments A ON A.AssignmentId = PA.AssignmentId
                            INNER JOIN Clubs C ON C.ClubId = A.ClubId
                            WHERE P.Guid = @PlayerGuid AND C.Guid = @ClubGuid AND A.Guid = @AssignmentGuid;";
            var parameters = new DynamicParameters();
            parameters.Add("@PlayerGuid", request.PlayerId);
            parameters.Add("@AssignmentGuid", request.AssignmentId);
            parameters.Add("@ClubGuid", request.ClubId);

            using (var connection = factory.Connect())
            {
                connection.Open();
                connection.Execute(sql, parameters);
            }
        }

		public void UpdateAssignment(AssignmentUpdateRequest request) {
			var sql = @"UPDATE Assignments SET 
                            DueDate = @DueDate, Instructions = @Instructions, Title = @Title
                        WHERE Guid = @AssignmentGuid AND ClubId = (SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid);
                        
                        DELETE AssignmentTrainingMaterials 
                        WHERE AssignmentId = (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid);
                    ";

			var parameters = new DynamicParameters();	
			parameters.Add("@AssignmentGuid", request.AssignmentId);
			parameters.Add("@ClubGuid", request.ClubId);
			parameters.Add("@Instructions", request.Instructions);
            parameters.Add("@Title", request.Title);
            parameters.Add("@DueDate", request.DueDate);
            parameters.Add("@DueDate", request.DueDate);

            using (var connection = factory.Connect()) {
				connection.Open();
                using(var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(sql, parameters, transaction);
                        AddTrainingMaterials(request.AssignmentId, request.TrainingMaterials, connection, transaction);
                        transaction.Commit();
                    }catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
				
			}
		}

		public void AddPlayerToAssignment(PlayerAssignmentRequest request) {
			var parameters = new DynamicParameters();
			parameters.Add("@AssignmentGuid", request.AssignmentId);
			parameters.Add("@PlayerGuid", request.PlayerId);
			parameters.Add("@CoachGuid", request.CoachId);
			parameters.Add("@ClubGuid", request.ClubId.ToString());
			var sql = $@"INSERT INTO PlayerAssignments (PlayerId, AssignmentId, AssignedOn, CoachId)
                        SELECT P.PlayerId,
                            (SELECT AssignmentId FROM Assignments WHERE Guid = @AssignmentGuid),
                            GetDate(),
                            (Select CoachId FROM Coaches WHERE Guid = @CoachGuid)
                        FROM Players P
                        WHERE P.Guid = @PlayerGuid;";

			using (var connection = factory.Connect()) 
			{
				connection.Open(); 
				connection.Execute(sql, parameters);
			}
		}
	}
}
