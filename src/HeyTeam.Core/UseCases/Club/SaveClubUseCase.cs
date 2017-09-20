using System;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Club {
    public class SaveClubUseCase : IUseCase<SaveClubRequest, SaveClubResponse>
    {
        private readonly IClubRepository repository;
        private readonly IValidator<SaveClubRequest> validator;

        public SaveClubUseCase (IClubRepository repository, IValidator<SaveClubRequest> validator) {
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }
        public SaveClubResponse Execute(SaveClubRequest request)
        {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid)
                return new SaveClubResponse(validationResult);
            
            Entities.Club club = MapClub(request);
            var savedClub = repository.Save(club);
            return new SaveClubResponse (validationResult, savedClub.Id);
        }

        private Entities.Club MapClub(SaveClubRequest request) 
            => new Entities.Club(request.ClubId) { 
                Name = request.ClubName, LogoUrl = request.ClubLogoUrl 
            };
    
    }
}