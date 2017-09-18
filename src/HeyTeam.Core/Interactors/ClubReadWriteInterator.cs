using System;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubReadWriteInteractor {
        private readonly IClubRepository repository;
        private readonly IValidator<SaveClubRequest> saveClubValidator;
        public ClubReadWriteInteractor (IClubRepository repository, IValidator<SaveClubRequest> saveClubValidator){
            if(repository ==null || saveClubValidator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.saveClubValidator = saveClubValidator;
        }

        public SaveClubResponse Handle(SaveClubRequest request) {
            var validationResult = saveClubValidator.Validate(request);
            if(!validationResult.IsValid) 
                return new SaveClubResponse(validationResult);

            var club = new Club(request.ClubId) { Name = request.ClubName };
            var savedClub = repository.Save(club);
            return new SaveClubResponse (validationResult, savedClub.Id);
        }

        public GetClubResponse Handle(GetClubRequest request) {
            var clubs = request.ClubId.HasValue ? repository.Get(request.ClubId.Value) : repository.Get(request.NameStartsWith);
            return new GetClubResponse(new ValidationResult<GetClubRequest>(request), clubs);
        }
    }
}