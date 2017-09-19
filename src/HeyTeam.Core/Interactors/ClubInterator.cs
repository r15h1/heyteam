using System;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Requests;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubInteractor {
        private readonly IClubRepository repository;
        private readonly IValidator<ClubSaveRequest> saveClubValidator;
        public ClubInteractor (IClubRepository repository, IValidator<ClubSaveRequest> saveClubValidator){
            if(repository ==null || saveClubValidator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.saveClubValidator = saveClubValidator;
        }

        public ClubSaveResponse Handle(ClubSaveRequest request) {
            var validationResult = saveClubValidator.Validate(request);
            if(!validationResult.IsValid) 
                return new ClubSaveResponse(validationResult);

            var club = new Club(request.ClubId) { Name = request.ClubName };
            var savedClub = repository.Save(club);
            return new ClubSaveResponse (validationResult, savedClub.Id);
        }

        public ClubGetResponse Handle(ClubGetRequest request) {
            var clubs = request.ClubId.HasValue ? repository.Get(request.ClubId.Value) : repository.Get(request.NameStartsWith);
            return new ClubGetResponse(new ValidationResult<ClubGetRequest>(request), clubs);
        }
    }
}