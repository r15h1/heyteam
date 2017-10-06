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

        public Response<Guid?> Execute(AddPlayerRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<Guid?>.CreateResponse(validationResult.Messages);

            var squad = squadRepository.Get(request.SquadId);
            if (squad == null)
                return Response<Guid?>.CreateResponse(new SquadNotFoundException());

            var player = MapPlayer(request);
            try {
                squad.AddPlayer(player);
                playerRepository.Add(player);
            } catch (Exception ex) {
                return Response<Guid?>.CreateResponse(ex);
            }        
            return new Response<Guid?>(player.Guid);;
        
        }

        private Entities.Player MapPlayer(AddPlayerRequest request) =>
        new Entities.Player(request.SquadId) {  
            DateOfBirth = request.DateOfBirth.Value,
            DominantFoot = request.DominantFoot,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Nationality = request.Nationality,
            Positions = request.Positions,
            SquadNumber = request.SquadNumber
        };        
    }
}