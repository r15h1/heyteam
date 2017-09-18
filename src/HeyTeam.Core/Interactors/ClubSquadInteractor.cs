using HeyTeam.Core.Entities;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.Interactors {
    public class ClubSquadInteractor {
        private readonly IClubRepository clubRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IValidator<ClubSquadRequest> validator;
        public ClubSquadInteractor (IClubRepository clubRepository, ISquadRepository squadRepository, IValidator<ClubSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.validator = validator;
        }

        public ClubSquadResponse SetupSquad(ClubSquadRequest request) {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid) 
                return new ClubSquadResponse(validationResult);

            var club = clubRepository.Get(request.ClubId.Value);
            var squad = new Squad(club) { Id = request.SquadId, Name = request.SquadName };
            try {
                club.AddSquad(squad);
            } catch (PolicyViolationException ex) {
                validationResult.AddMessage(ex.Message);
                return new ClubSquadResponse(validationResult);
            }
            
            Squad savedSquad = squadRepository.Save(squad);
            return new ClubSquadResponse(validationResult, savedSquad.Id);
        }
    }
}