using System;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubReadWriteInteractor {
        private readonly IClubRepository repository;
        private readonly IValidator<SaveClubRequest> validator;
        public ClubReadWriteInteractor (IClubRepository repository, IValidator<SaveClubRequest> validator){
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }

        public SaveClubResponse Handle(SaveClubRequest request) {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid) 
                return new SaveClubResponse(validationResult);

            var club = new Club(request.ClubId) { Name = request.ClubName };
            var savedClub = repository.Save(club);
            return new SaveClubResponse (validationResult, savedClub.Id);
        }

        
    }
}