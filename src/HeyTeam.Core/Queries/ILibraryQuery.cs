using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface ILibraryQuery
    {
		IEnumerable<TrainingMaterial> GetTrainingMaterials(Guid clubId);
		IEnumerable<dynamic> Search(Guid clubId, string searchTerm, int page = 1, int limit = 10);
		TrainingMaterial GetTrainingMaterial(Guid trainingMaterialId);
		void UpdateCache(Guid clubId);
	}
}
