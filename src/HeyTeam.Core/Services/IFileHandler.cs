using System;
using System.IO;

namespace HeyTeam.Core.Services {
	public interface IFileHandlerFactory {
		IFileHandler GetFileHandler(string contentType);
	}

	public interface IFileHandler
    {
		(UploadResult Result, Response Response) Upload(UploadRequest request);
		(Metadata Metadata, Response Response) GetMetaData(TrainingMaterial trainingMaterial);
    }

	public class UploadRequest {
		public Stream Stream { get; set; }
		public string ContentType { get; set; }
		public string OriginalFileName { get; set; }
		public string Title{ get; set; }
		public string Description { get; set; }

	}

	public class UploadResult {
		public UploadResult(string id, string url, string thumbnailUrl) {
			this.Id = id;
			this.Url = url;
			this.ThumbnailUrl = thumbnailUrl;
		}
		public string Id { get; }
		public string Url { get; }
		public string ThumbnailUrl { get; }
	}

	public class MetadataRequest {
		public Guid ClubId{ get; set; }
		public Guid TrainingMaterialId { get; set; }		
	}

	public class Metadata {
		public string Status { get; set; }
		public string Url{ get; set; }
		public string Thumbnail { get; set; }
	}
}