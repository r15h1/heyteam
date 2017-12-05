using System;
using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Repositories;
using HeyTeam.Lib.Data;
using HeyTeam.Util;

namespace HeyTeam.Lib.Repositories {
	public class LibraryRepository : ILibraryRepository {
		private readonly IDbConnectionFactory connectionFactory;

		public LibraryRepository(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public void AddTrainingMaterial(TrainingMaterial trainingMaterial) {
			ThrowIf.ArgumentIsNull(trainingMaterial);
			using (var connection = connectionFactory.Connect()) {
				string sql = GetInsertStatement();
				DynamicParameters p = SetupInsertParameters(trainingMaterial);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		private DynamicParameters SetupInsertParameters(TrainingMaterial material) {
			var p = new DynamicParameters();
			p.Add("@ClubGuid", material.ClubId.ToString());
			p.Add("@TrainingMaterialGuid", material.Guid.ToString());
			p.Add("@Title", material.Title);
			p.Add("@Description", material.Description);
			p.Add("@ContentType", material.ContentType);
			p.Add("@ExternalId", material.ExternalId);
			p.Add("@Url", material.Url);
			p.Add("@ThumbnailUrl", material.ThumbnailUrl);
			return p;
		}

		private string GetInsertStatement() =>
			"INSERT INTO TrainingMaterials (ClubId, Guid, Title, Description, ContentType, ExternalId, Url, ThumbnailUrl) " +
			"VALUES ((SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid)," +
			"@TrainingMaterialGuid, @Title, @Description, @ContentType, @ExternalId, @Url, @ThumbnailUrl)";

		public void UpdateTrainingMaterial(TrainingMaterial trainingMaterial) {
			ThrowIf.ArgumentIsNull(trainingMaterial);
			using (var connection = connectionFactory.Connect()) {
				string sql = GetUpdateStatement();
				DynamicParameters p = SetupUpdateParameters(trainingMaterial);
				connection.Open();
				connection.Execute(sql, p);
			}
		}

		private string GetUpdateStatement() => "UPDATE TrainingMaterials SET " +
			"Title = @Title, Description = @Description, ContentType = @ContentType, Url = @Url, ThumbnailUrl = @ThumbnailUrl " +
			"WHERE Guid = @TrainingMaterialGuid";

		private DynamicParameters SetupUpdateParameters(TrainingMaterial trainingMaterial) {
			var p = new DynamicParameters();
			p.Add("@TrainingMaterialGuid", trainingMaterial.Guid.ToString());
			p.Add("@Title", trainingMaterial.Title);
			p.Add("@Description", trainingMaterial.Description);
			p.Add("@ContentType", trainingMaterial.ContentType);
			p.Add("@ExternalId", trainingMaterial.ExternalId);
			p.Add("@Url", trainingMaterial.Url);
			p.Add("@ThumbnailUrl", trainingMaterial.ThumbnailUrl);
			return p;
		}

		public void DeleteTrainingMaterial(Guid ClubId, Guid trainingMaterialId) {			
			using (var connection = connectionFactory.Connect()) {
				string sql = "DELETE TrainingMaterials WHERE Guid = @TrainingMaterialGuid AND ClubId = (SELECT ClubId FROM Clubs WHERE Guid = @ClubGuid)";
				var p = new DynamicParameters();
				p.Add("@ClubGuid", ClubId.ToString());
				p.Add("@TrainingMaterialGuid", trainingMaterialId.ToString());
				connection.Open();
				connection.Execute(sql, p);
			}
		}
	}
}
