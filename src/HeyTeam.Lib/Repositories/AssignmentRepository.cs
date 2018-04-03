using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using System;

namespace HeyTeam.Lib.Repositories {
	public class AssignmentRepository : IAssignmentRepository {
		private readonly IDbConnectionFactory factory;

		public AssignmentRepository(IDbConnectionFactory factory){
			this.factory = factory;
		}

		public void CreateAssignment(AssignmentRequest request) {
			var sql = "";
			var parameters = new DynamicParameters();
			using(var connection = factory.Connect()){
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						connection.Execute(sql, parameters, transaction);
						transaction.Commit();
					} catch(Exception ex){
						transaction.Rollback();
						throw ex;
					}
				}
			}
		}
	}
}
