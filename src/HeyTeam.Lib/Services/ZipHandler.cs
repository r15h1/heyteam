using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using HeyTeam.Core;
using HeyTeam.Lib.Settings;
using System.IO;
using HeyTeam.Core.Exceptions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace HeyTeam.Lib.Services
{
	public class ZipHandler : IFileHandler {
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
			{".zip", "/images/ssp.png"},
		};


		public ZipHandler(FileConfiguration config){
			this.config = config;
		}
		public (UploadResult Result, Response Response) Upload(UploadRequest request) {
			if (request == null)
				return (null, Response.CreateResponse(new ArgumentNullException("request", "Request cannot be null")));
			else if (request.Stream == null)
				return (null, Response.CreateResponse(new IllegalOperationException("The uploaded file is invalid")));

			var root = config.FileSettings.Directory+ $"\\{DateTime.Today.ToString("yyyy-MM")}";
			var subfolder = $"{DateTime.Now.ToString("dd-hh-mm")}";
			int counter = 1;
			while(Directory.Exists($"{root}\\{subfolder}")) {
				counter++;
				subfolder = $"{DateTime.Now.ToString("dd-hh-mm")}-{counter}";
			}

			Directory.CreateDirectory($"{root}\\{subfolder}");
			
			var (filename, thumbnail) = GenerateFileNameAndThumbnail($"{root}\\{subfolder}", request.OriginalFileName);
			using (var fileStream = new FileStream(Path.Combine($"{root}\\{subfolder}", filename), FileMode.Create)) {
				Task.WaitAll(request.Stream.CopyToAsync(fileStream));				

				Unzip($"{root}\\{subfolder}", fileStream);
				var files = Directory.GetFileSystemEntries($"{root}\\{subfolder}", "*.html");
				string url = $"{config.FileSettings.RootUrl}{DateTime.Today.ToString("yyyy-MM")}/{subfolder}/";
				if (files.Length > 0) {
					FileInfo info = new FileInfo(files[0]);
					url = url + info.Name;
				}
				return (new UploadResult("", url, thumbnail), Response.CreateSuccessResponse());
			}			
		}

		private void Unzip(string location, FileStream fileStream) {
			ZipFile zf = null;
			try {
				zf = new ZipFile(fileStream);
				foreach (ZipEntry zipEntry in zf) {
					if (!zipEntry.IsFile) {
						continue;           // Ignore directories
					}
					String entryFileName = zipEntry.Name;
					// to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
					// Optionally match entrynames against a selection list here to skip as desired.
					// The unpacked length is available in the zipEntry.Size property.

					byte[] buffer = new byte[4096];     // 4K is optimum
					Stream zipStream = zf.GetInputStream(zipEntry);

					// Manipulate the output filename here as desired.
					String fullZipToPath = Path.Combine(location, entryFileName);
					string directoryName = Path.GetDirectoryName(fullZipToPath);
					if (directoryName.Length > 0)
						Directory.CreateDirectory(directoryName);

					// Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
					// of the file, but does not waste memory.
					// The "using" will close the stream even if an exception occurs.
					using (FileStream streamWriter = File.Create(fullZipToPath)) {
						StreamUtils.Copy(zipStream, streamWriter, buffer);
					}
				}
			} finally {
				if (zf != null) {
					zf.IsStreamOwner = true; // Makes close also shut the underlying stream
					zf.Close(); // Ensure we release resources
				}
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
