using System;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubSetupInteractor {
        private IClubRepository repository;
        private IValidator<ClubSetupRequest> validator;
        public ClubSetupInteractor (IClubRepository repository, IValidator<ClubSetupRequest> validator){
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }

        public ClubSetupResponse SetupClub(ClubSetupRequest request) {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid) 
                return new ClubSetupResponse(validationResult);

            var club = new Club(request.ClubId) { Name = request.ClubName };
            var savedClub = repository.Save(club);
            return new ClubSetupResponse (validationResult, savedClub.Id);
        }
    }
}