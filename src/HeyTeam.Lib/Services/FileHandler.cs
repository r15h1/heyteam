using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using HeyTeam.Core;
using HeyTeam.Lib.Settings;
using System.IO;
using HeyTeam.Core.Exceptions;
using System.Threading.Tasks;

namespace HeyTeam.Lib.Services
{
	public class FileHandler : IFileHandler {
		private readonly FileConfiguration config;
		private readonly Dictionary<string, string> thumbnails = new Dictionary<string, string> { 
			{".doc", "/images/doc.png"},
			{".docx", "/images/doc.png"},
			{".txt", "/images/txt.png"},
			{".jpeg", "/images/jpeg.png"},
			{".jpg", "/images/jpeg.png"},
			{".pdf", "/images/pdf.png"},
			{"na", "/images/na.png"},
			{"file", "/images/file.png"},
		};


		public FileHandler(FileConfiguration config){
			this.config = config;
		}
		public (UploadResult Result, Response Response) Upload(UploadRequest request) {
			if (request == null)
				return (null, Response.CreateResponse(new ArgumentNullException("request", "Request cannot be null")));
			else if (request.Stream == null)
				return (null, Response.CreateResponse(new IllegalOperationException("The uploaded file is invalid")));

			var location = config.FileSettings.Directory+ $"\\{DateTime.Today.ToString("yyyy-MM")}";
			if (!Directory.Exists(location)) Directory.CreateDirectory(location);
			
			var (filename, thumbnail) = GenerateFileNameAndThumbnail(location, request.OriginalFileName);
			using (var fileStream = new FileStream(Path.Combine(location, filename), FileMode.Create)) {
				Task.WaitAll(request.Stream.CopyToAsync(fileStream));
				var url = $"{config.FileSettings.RootUrl}{DateTime.Today.ToString("yyyy-MM")}/{filename}";
				return (new UploadResult("", url, thumbnail), Response.CreateSuccessResponse());
			}			
		}

		private (string, string) GenerateFileNameAndThumbnail(string location, string fileName) {
					
			int counter = 1;
			string tempFileName = fileName;
			while (System.IO.File.Exists(Path.Combine(location, tempFileName))) {
				tempFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + counter + Path.GetExtension(fileName);
				counter++;
			}
			var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
			string thumbnail = thumbnails.ContainsKey(fileExtension) ? thumbnails[fileExtension] : thumbnails["file"]; 
			return (tempFileName, thumbnail);
		}

		public (Metadata Metadata, Response Response) GetMetaData(TrainingMaterial trainingMaterial) {
			throw new NotImplementedException();
		}
	}
}
