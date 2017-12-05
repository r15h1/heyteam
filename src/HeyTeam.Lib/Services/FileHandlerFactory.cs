using HeyTeam.Core.Services;
using HeyTeam.Lib.Settings;
using HeyTeam.Util;
using Microsoft.Extensions.Options;

namespace HeyTeam.Lib.Services {
	public class FileHandlerFactory:IFileHandlerFactory
    {
		private readonly VideoConfiguration videoConfig;

		public FileHandlerFactory(IOptions<VideoConfiguration> videoConfig) {
			this.videoConfig = videoConfig.Value;
		}
		public IFileHandler GetFileHandler(string contentType) =>
			(!contentType.IsEmpty() && contentType.ToLowerInvariant().Contains("video")) ?
				new VideoHandler(videoConfig) : new FileHandler() as IFileHandler;
    }
}