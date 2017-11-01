using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using static HeyTeam.Core.UseCases.Club.UpdateClubProfileRequest;

namespace HeyTeam.Core.UseCases.Club {
    public class UpdateClubProfileUseCase : IUseCase<UpdateClubProfileRequest, Response<Guid>>
    {
        private readonly IClubRepository repository;
        private readonly IValidator<UpdateClubProfileRequest> validator;

        public UpdateClubProfileUseCase(IClubRepository repository, IValidator<UpdateClubProfileRequest> validator) {
            Ensure.ArgumentNotNull(repository);
            Ensure.ArgumentNotNull(validator);
            this.repository = repository;
            this.validator = validator;
        }

        public Response<Guid> Execute(UpdateClubProfileRequest request)
        {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid) 
                return CreateResponse(validationResult);

            var club = repository.GetClub(request.ClubId);
            if (club == null) 
                return CreateResponse(new ClubNotFoundException(), "The specified club does not exist");

            if(request.Fields.ContainsKey(UpdatableFields.URL))
                if (repository.IsUrlAlreadyAssigned(request.Fields[UpdatableFields.URL], request.ClubId))
                    return Response<Guid>.CreateResponse(new DuplicateEntryException("This url has already been used."));

            club.Url = request.Fields.ContainsKey(UpdatableFields.URL) ? request.Fields[UpdatableFields.URL] : club.Url;
            club.Name = request.Fields.ContainsKey(UpdatableFields.NAME) ? request.Fields[UpdatableFields.NAME] : club.Name;
            repository.UpdateClub(club);

            return new Response<Guid>(); 
        }

        private Response<Guid> CreateResponse(Exception exception, string message)
        {
            var response = new Response<Guid>(exception);
            response.AddError(message);
            return response; 
        }

        private Response<Guid> CreateResponse(ValidationResult<UpdateClubProfileRequest> validationResult)
        {
            var response = new Response<Guid>();
            validationResult.Messages.ForEach((m) => response.AddError(m));
            return response;
        }
    }
}