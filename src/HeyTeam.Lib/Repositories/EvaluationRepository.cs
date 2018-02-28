using Dapper;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Repositories {
	public class EvaluationRepository : IEvaluationRepository {
		private readonly IDbConnectionFactory connectionFactory;
		public EvaluationRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddTerm(TermSetupRequest request) {
			ThrowIf.ArgumentIsNull(request);
			string sql = @"INSERT INTO EvaluationTerms (Guid, ClubId, Title, TermStatusId, StartDate, EndDate)
							VALUES(	
									@TermGuid, 
									(SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid),
									@Title, 1, @StartDate, @EndDate							
							)";

			
			DynamicParameters p = new DynamicParameters();
			p.Add("@TermGuid", Guid.NewGuid().ToString());

			using (var connection = connectionFactory.Connect()) {				
				
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		public void DeleteTerm(TermDeleteRequest request) {
			throw new System.NotImplementedException();
		}

		public void UpdateTerm(TermSetupRequest request) {
			throw new System.NotImplementedException();
		}
	}
}
