using HeyTeam.Core.Services;
using HeyTeam.Lib.Settings;
using HeyTeam.Util;
using Microsoft.Extensions.Options;

namespace HeyTeam.Lib.Services {
	public class FileHandlerFactory:IFileHandlerFactory
    {
		private readonly VideoConfiguration videoConfig;
		private readonly FileConfiguration fileConfig;

		public FileHandlerFactory(IOptions<VideoConfiguration> videoConfig, IOptions<FileConfiguration> fileConfig) {
			this.videoConfig = videoConfig.Value;
			this.fileConfig = fileConfig.Value;
		}
		public IFileHandler GetFileHandler(string contentType) {
			if (!contentType.IsEmpty()) {
				if (contentType.ToLowerInvariant().Contains("video")) return new VideoHandler(videoConfig);
				else if (contentType.ToLowerInvariant().Contains("zip")) return new ZipHandler(fileConfig);
			}

			return new FileHandler(fileConfig) as IFileHandler;
		}
    }
}