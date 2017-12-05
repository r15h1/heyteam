using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Services;
using HeyTeam.Lib.Settings;
using System;
using System.Threading.Tasks;
using VimeoDotNet;
using VimeoDotNet.Enums;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace HeyTeam.Lib.Services {
	public class VideoHandler : IFileHandler {
		private readonly VideoConfiguration config;

		public VideoHandler(VideoConfiguration config){
			this.config = config;
		}

		public (Metadata Metadata, Response Response) GetMetaData(TrainingMaterial trainingMaterial) {
			long clipId = 0;
			var client = new VimeoClient(config.VideoSettings.Token);

			if (!long.TryParse(trainingMaterial.ExternalId, out clipId))
				return (null, Response.CreateResponse(new IllegalOperationException("The training material id could not be converted into the desired format (string to long)")));

			try {
				var video = client.GetVideoAsync(clipId).Result;
				if(video == null || video.id != clipId)
					return (null, Response.CreateResponse(new EntityNotFoundException("The desired video could not be retrieved from the host service (Vimeo)")));

				return (new Metadata { Status = video.status, Thumbnail = (video.pictures?.sizes?.Count > 0 ? video.pictures.sizes[0].link : string.Empty), Url = video.StandardVideoLink  }, Response.CreateSuccessResponse());
			} catch (Exception ex) {
				return (null, Response.CreateResponse(ex));
			}
		}

		public (UploadResult Result, Response Response) Upload(Core.Services.UploadRequest request) {
			try {
				var client = new VimeoClient(config.VideoSettings.Token);
				var ticket = client.GetUploadTicketAsync().Result;
				IBinaryContent file = new BinaryContent(request.Stream, request.ContentType);
				var uploadRequest = client.UploadEntireFileAsync(file).Result;
				var verificationResult = client.VerifyUploadFileAsync(uploadRequest).Result;

				Task.WaitAll(client.UpdateVideoMetadataAsync(uploadRequest.ClipId.Value,
					new VideoUpdateMetadata {						
						Description = request.Description, Name = request.Title,
						Privacy = VideoPrivacyEnum.Password, AllowDownloadVideo = false, Password = config.VideoSettings.Password
					})
				);

				var albumResult = client.AddToAlbumAsync(4877366, uploadRequest.ClipId.Value).Result;
				Task.WaitAll(client.CompleteFileUploadAsync(uploadRequest));
				var video = client.GetVideoAsync(uploadRequest.ClipId.Value).Result;
				string thumbnailUrl = null;
				if (video?.pictures?.sizes != null && video.pictures.sizes.Count > 0)
					thumbnailUrl = video.pictures.sizes[0].link;

				return (new UploadResult(uploadRequest.ClipId.ToString(), video.StandardVideoLink, thumbnailUrl), Response.CreateSuccessResponse());
			}catch (Exception ex) {
				return (null, Response.CreateResponse(ex));
			}
		}		
	}
}
