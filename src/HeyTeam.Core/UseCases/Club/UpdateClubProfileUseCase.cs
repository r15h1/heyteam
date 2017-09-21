using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class UpdateClubProfileUseCase : IUseCase<UpdateClubProfileRequest, UpdateClubProfileResponse>
    {
        private readonly IClubRepository repository;
        private readonly IValidator<UpdateClubProfileRequest> validator;

        public UpdateClubProfileUseCase(IClubRepository repository, IValidator<UpdateClubProfileRequest> validator) {
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }

        public UpdateClubProfileResponse Execute(UpdateClubProfileRequest request)
        {
            var result = validator.Validate(request);
            if(!result.IsValid) 
                return new UpdateClubProfileResponse(result);

            var club = repository.Get(request.ClubId);
            if (club == null) 
                throw new ClubNotFoundException();

            club.LogoUrl = request.ClubLogoUrl;
            club.Name = request.ClubName;
            repository.Update(club);

            return new UpdateClubProfileResponse(result); 
        }
    }
}