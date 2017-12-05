using HeyTeam.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using HeyTeam.Core;

namespace HeyTeam.Lib.Services
{
	public class FileHandler : IFileHandler {
		public (UploadResult Result, Response Response) Upload(UploadRequest request) {
			throw new NotImplementedException();
		}

		public (Metadata Metadata, Response Response) GetMetaData(TrainingMaterial trainingMaterial) {
			throw new NotImplementedException();
		}
	}
}
