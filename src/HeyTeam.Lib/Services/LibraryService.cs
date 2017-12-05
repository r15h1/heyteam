using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Exceptions;
using HeyTeam.Util;

namespace HeyTeam.Lib.Services {
	public class LibraryService : ILibraryService {
		private readonly IValidator<TrainingMaterialRequest> trainingMaterialRequestValidator;
		private readonly IValidator<ReSyncRequest> resyncRequestValidator;
		private readonly IFileHandlerFactory fileHandlerFactory;
		private readonly ILibraryRepository libraryRepository;
		private readonly ILibraryQuery libraryQuery;

		public LibraryService(	IValidator<TrainingMaterialRequest> trainingMaterialRequestValidator,
								IValidator<ReSyncRequest> resyncRequestValidator,
								IFileHandlerFactory fileHandlerFactory, 
								ILibraryRepository libraryRepository, 
								ILibraryQuery libraryQuery){
			this.trainingMaterialRequestValidator = trainingMaterialRequestValidator;
			this.resyncRequestValidator = resyncRequestValidator;
			this.fileHandlerFactory = fileHandlerFactory;
			this.libraryRepository = libraryRepository;
			this.libraryQuery = libraryQuery;
		}
		
		public Response AddTrainingMaterial(TrainingMaterialRequest request) {
			var validationResult = trainingMaterialRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var handler = fileHandlerFactory.GetFileHandler(request.ContentType);
			var uploadResponse = handler.Upload(new UploadRequest {
				ContentType = request.ContentType,
				OriginalFileName = request.OriginalFileName,
				Stream = request.Stream,
				Description = request.Description,
				Title = request.Title
			});

			if (!uploadResponse.Response.RequestIsFulfilled)
				return Response.CreateResponse(uploadResponse.Response.Errors);

			TrainingMaterial material = MapTrainingMaterial(request, uploadResponse.Result);
			try {
				libraryRepository.AddTrainingMaterial(material);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		public Response DeleteTrainingMaterial(TrainingMaterialDeleteRequest request) {
			if(request == null || request.ClubId.IsEmpty() || request.TrainingMaterialId.IsEmpty())			
				return Response.CreateResponse(new ArgumentException("Request, ClubId and TrainingMaterialId must be valid"));

			var trainingMaterial = libraryQuery.GetTrainingMaterial(request.TrainingMaterialId);
			if (trainingMaterial == null)
				return Response.CreateResponse(new EntityNotFoundException());
			else if (trainingMaterial.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified training material does not belong to this club"));

			try {				
				libraryRepository.DeleteTrainingMaterial(trainingMaterial.ClubId, trainingMaterial.Guid.Value);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		public Response ReSync(ReSyncRequest request) {
			var validationResult = resyncRequestValidator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var trainingMaterial = libraryQuery.GetTrainingMaterial(request.TrainingMaterialId);
			if (trainingMaterial == null)
				return Response.CreateResponse(new EntityNotFoundException());
			else if (trainingMaterial.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified training material does not belong to this club"));

			var fileHandler = fileHandlerFactory.GetFileHandler(trainingMaterial.ContentType);
			var (metadata, response) = fileHandler.GetMetaData(trainingMaterial);
			if (!response.RequestIsFulfilled)
				return response;

			try {
				trainingMaterial.ThumbnailUrl = metadata.Thumbnail;
				trainingMaterial.Url = metadata.Url;
				libraryRepository.UpdateTrainingMaterial(trainingMaterial);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		private TrainingMaterial MapTrainingMaterial(TrainingMaterialRequest request, UploadResult uploadResult) =>
			new TrainingMaterial(request.ClubId) {
				ContentType = request.ContentType,
				Description = request.Description,
				ExternalId = uploadResult.Id,
				Title = request.Title,
				Url = uploadResult.Url,
				ThumbnailUrl = uploadResult.ThumbnailUrl
			};
		
	}
}
