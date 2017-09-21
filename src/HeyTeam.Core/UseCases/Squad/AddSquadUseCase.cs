using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

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

            return new AddSquadResponse(validationResult);
        }
    }
}