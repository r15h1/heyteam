using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Queries
{
    public interface ILibraryQuery
    {
		IEnumerable<TrainingMaterial> GetTrainingMaterials(Guid clubId);
		TrainingMaterial GetTrainingMaterial(Guid trainingMaterialId);
	}
}
