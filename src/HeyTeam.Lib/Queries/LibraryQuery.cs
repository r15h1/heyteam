using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class LibraryQuery : ILibraryQuery {
		private readonly IDbConnectionFactory connectionFactory;

		public LibraryQuery(IDbConnectionFactory factory) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
		}

		public TrainingMaterial GetTrainingMaterial(Guid trainingMaterialId) {
			if (trainingMaterialId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"	SELECT T.Guid AS TrainingMaterialGuid, C.Guid AS ClubGuid, T.Title, T.Description, T.ContentType, T.ExternalId, T.Url, T.ThumbnailUrl
								FROM TrainingMaterials T
								INNER JOIN Clubs C ON C.ClubId = T.ClubId AND T.Guid = @TrainingMaterialGuid";
				DynamicParameters p = new DynamicParameters();
				p.Add("@TrainingMaterialGuid", trainingMaterialId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var material = reader.Select<dynamic, TrainingMaterial>(
						row => new TrainingMaterial(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.TrainingMaterialGuid.ToString())) {
							ContentType = row.ContentType, Description = row.Description, ExternalId = row.ExternalId,
							ThumbnailUrl = row.ThumbnailUrl, Title = row.Title, Url = row.Url
						}).FirstOrDefault();

				return material;
			}
		}

		public IEnumerable<TrainingMaterial> GetTrainingMaterials(Guid clubId) {
			if (clubId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"	SELECT T.Guid AS TrainingMaterialGuid, C.Guid AS ClubGuid, T.Title, T.Description, T.ContentType, T.ExternalId, T.Url, T.ThumbnailUrl
								FROM TrainingMaterials T
								INNER JOIN Clubs C ON C.ClubId = T.ClubId AND C.Guid = @ClubGuid								
								ORDER BY T.TrainingMaterialId DESC";
				DynamicParameters p = new DynamicParameters();
				p.Add("@ClubGuid", clubId.ToString());
				connection.Open();
				var reader = connection.Query(sql, p).Cast<IDictionary<string, object>>();
				var list = reader.Select<dynamic, TrainingMaterial>(
						row => new TrainingMaterial(Guid.Parse(row.ClubGuid.ToString()), Guid.Parse(row.TrainingMaterialGuid.ToString())) {
							ContentType = row.ContentType, Description = row.Description, ExternalId = row.ExternalId, 
							ThumbnailUrl = row.ThumbnailUrl, Title = row.Title, Url = row.Url
						}).ToList();

				return list;
			}
		}
	}
}
