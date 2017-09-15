using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validators;

namespace HeyTeam.Core.Interactors {
    public class SquadChief {
        private ISquadRepository repository;
        private ISquadValidator validator;
        public SquadChief (ISquadRepository repository, ISquadValidator validator) {
            this.repository = repository;
            this.validator = validator;
        }
    }

    //public void CreateOrUpdateSquad
}