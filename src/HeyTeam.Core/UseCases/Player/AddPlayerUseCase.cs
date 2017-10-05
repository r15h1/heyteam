using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Player {
    public class AddPlayerUseCase : IUseCase<AddPlayerRequest, Response<Guid?>>
    {   
        private readonly IPlayerRepository playerRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IValidator<AddPlayerRequest> validator;

        public AddPlayerUseCase(ISquadRepository squadRepository, IPlayerRepository playerRepository, IValidator<AddPlayerRequest> validator) {
            this.squadRepository = squadRepository;
            this.playerRepository = playerRepository;
            this.validator = validator;
        }

        Response<Guid?> IUseCase<AddPlayerRequest, Response<Guid?>>.Execute(AddPlayerRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<Guid?>.CreateResponse(validationResult.Messages);

            var squad = squadRepository.Get(request.SquadId);
            if (squad == null)
                return Response<Guid?>.CreateResponse(new SquadNotFoundException());

            var player = new Entities.Player(squad.Guid) {  };
            return new Response<Guid?>(player.Guid);;
        }
    }
}