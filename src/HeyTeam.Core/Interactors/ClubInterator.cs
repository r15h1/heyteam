using System;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubInteractor {
        private IClubRepository repository;
        private IValidator<Squad> validator;
        public ClubInteractor (IClubRepository repository, IValidator<Squad> validator){
            if(repository ==null || validator == null) throw new ArgumentNullException();
            this.repository = repository;
            this.validator = validator;
        }

        public SquadCreationResponse CreateSquad(SquadCreationRequest request) {
            throw new NotImplementedException();
        }
    }
}