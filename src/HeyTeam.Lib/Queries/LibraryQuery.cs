using Dapper;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Lib.Data;
using HeyTeam.Util;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Lib.Queries {
	public class LibraryQuery : ILibraryQuery {
		private readonly IDbConnectionFactory connectionFactory;
		private readonly IMemoryCache cache;

		public LibraryQuery(IDbConnectionFactory factory, IMemoryCache cache) {
			ThrowIf.ArgumentIsNull(factory);
			this.connectionFactory = factory;
			this.cache = cache;
		}

		public TrainingMaterial GetTrainingMaterial(Guid trainingMaterialId) {
			if (trainingMaterialId.IsEmpty())
				return null;

			using (var connection = connectionFactory.Connect()) {
				string sql = @"	SELECT T.Guid AS TrainingMaterialGuid, C.Guid AS ClubGuid, T.Title, T.Description, T.ContentType, T.ExternalId, T.Url, T.ThumbnailUrl
								FROM TrainingMaterials T
								INNER JOIN Clubs C ON C.ClubId = T.ClubId AND T.Guid = @TrainingMaterialGuid
								WHERE T.Deleted IS NULL OR T.Deleted = 0";
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
								WHERE T.Deleted IS NULL OR T.Deleted = 0
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

		public IEnumerable<dynamic> Search(Guid clubId, string searchTerm, int page = 1, int limit = 10) {
			IEnumerable<dynamic> list = null;
			if (!cache.TryGetValue<IEnumerable<dynamic>>(CacheKeys.LIBRARY_TRAINING_MATERIALS, out list)) {
				UpdateCache(clubId);
				list = cache.Get<IEnumerable<dynamic>>(CacheKeys.LIBRARY_TRAINING_MATERIALS);
			}

			return list?.Where(t => t.LowerTitle.Contains(searchTerm.ToLowerInvariant()))
				.Skip((page - 1) * limit)
				.Take(limit)
				.Select(t => new { Id = t.Id, Text = t.Title, Thumbnail = t.Thumbnail, ContentType = t.ContentType });
		}

		public void UpdateCache(Guid clubId) {
			var list = GetTrainingMaterials(clubId).Select(t => new { 
				LowerTitle = $"{t.Title} {t.ContentType}".ToLowerInvariant(),
				Title = t.Title,
				ContentType = t.ShortContentType,
				Thumbnail = t.ThumbnailUrl,
				Id = t.Guid
			});
			cache.Remove(CacheKeys.LIBRARY_TRAINING_MATERIALS);
			MemoryCacheEntryOptions options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(10) };
			cache.Set(CacheKeys.LIBRARY_TRAINING_MATERIALS, list, options);
		}
	}
}