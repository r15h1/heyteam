using System;
using System.IO;

namespace HeyTeam.Core.Services {
	public interface ILibraryService {
		Response AddTrainingMaterial(TrainingMaterialRequest request);
		Response ReSync(ReSyncRequest request);
		Response DeleteTrainingMaterial(TrainingMaterialDeleteRequest request);
	}

	public class TrainingMaterialRequest {
		public Guid? TrainingMaterialId { get; set; }
		public Guid ClubId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Stream Stream { get; set; }
		public string ContentType { get; set; }
		public string OriginalFileName { get; set; }
	}

	public class ReSyncRequest {
		public Guid TrainingMaterialId { get; set; }
		public Guid ClubId { get; set; }
	}

	public class TrainingMaterialDeleteRequest {
		public Guid TrainingMaterialId { get; set; }
		public Guid ClubId { get; set; }
	}
}