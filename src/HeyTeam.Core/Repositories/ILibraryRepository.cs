using System;
using System.Collections.Generic;
using System.Text;

namespace HeyTeam.Core.Repositories
{
    public interface ILibraryRepository
    {
		void AddTrainingMaterial(TrainingMaterial trainingMaterial);
		void UpdateTrainingMaterial(TrainingMaterial trainingMaterial);
		void DeleteTrainingMaterial(Guid ClubId, Guid trainingMaterialId);
	}
}
