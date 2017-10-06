using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

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

            var club = repository.Get(request.ClubId);
            if (club == null) 
                return CreateResponse(new ClubNotFoundException(), "The specified club does not exist");

            club.LogoUrl = request.ClubLogoUrl;
            club.Name = request.ClubName;
            repository.Update(club);

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