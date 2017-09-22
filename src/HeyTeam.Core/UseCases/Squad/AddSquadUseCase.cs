using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using System.Linq;

namespace HeyTeam.Core.UseCases.Squad {
    public class AddSquadUseCase : IUseCase<AddSquadRequest, AddSquadResponse>
    {
        private readonly IClubRepository clubRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IValidator<AddSquadRequest> validator;

        public AddSquadUseCase(IClubRepository clubRepository, ISquadRepository squadRepository, IValidator<AddSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.validator = validator;
        }
        public AddSquadResponse Execute(AddSquadRequest request)
        {
            var validationResult = validator.Validate(request);
            if(!validationResult.IsValid)
                return new AddSquadResponse(validationResult);

            var club = clubRepository.Get(request.ClubId);
            if(club == null) 
                throw new ClubNotFoundException();
            
            var squad = new Entities.Squad(club) { Name = request.SquadName };
            club.AddSquad(squad);
            squadRepository.Add(squad);

            return new AddSquadResponse(validationResult, squad.Id);
        }
    }
}