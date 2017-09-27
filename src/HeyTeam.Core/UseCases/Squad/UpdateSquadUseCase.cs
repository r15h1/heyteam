using System.Linq;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Core.UseCases.Squad {
    public class UpdateSquadUseCase : IUseCase<UpdateSquadRequest, UpdateSquadResponse>
    {
        private IClubRepository clubRepository;
        private ISquadRepository squadRepository;
        private IValidator<UpdateSquadRequest> validator;

        public UpdateSquadUseCase(IClubRepository clubRepository, ISquadRepository squadRepository, IValidator<UpdateSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.validator = validator;
        }

        public UpdateSquadResponse Execute(UpdateSquadRequest request) {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return new UpdateSquadResponse(validationResult);

            var club = clubRepository.Get(request.ClubId);
            Ensure.NotNull<Entities.Club, ClubNotFoundException>(club);

            var squad = new Entities.Squad(club, request.SquadId) { Name = request.SquadName };            
            club.UpdateSquad(squad);
            squadRepository.Update(squad);
            return new UpdateSquadResponse(validationResult);
        }        
    }
}