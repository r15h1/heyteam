using System;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;

namespace HeyTeam.Core.UseCases.Player {
    public class UpdatePlayerUseCase : IUseCase<UpdatePlayerRequest, Response<Guid?>>
    {   
        private readonly IPlayerRepository playerRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IValidator<UpdatePlayerRequest> validator;

        public UpdatePlayerUseCase(ISquadRepository squadRepository, IPlayerRepository playerRepository, IValidator<UpdatePlayerRequest> validator) {
            this.squadRepository = squadRepository;
            this.playerRepository = playerRepository;
            this.validator = validator;
        }

        public Response<Guid?> Execute(UpdatePlayerRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<Guid?>.CreateResponse(validationResult.Messages);

            var squad = squadRepository.GetSquad(request.SquadId);
            if (squad == null)
                return Response<Guid?>.CreateResponse(new EntityNotFoundException());            

            var player = MapPlayer(request);
            var playerWithSameId = playerRepository.GetPlayer(player.Guid);
            if(playerWithSameId == null)
                return Response<Guid?>.CreateResponse(new EntityNotFoundException("A player with the specified id was not found"));

            try {   
                playerRepository.UpdatePlayer(player);
            } catch (Exception ex) {
                return Response<Guid?>.CreateResponse(ex);
            }        
            return new Response<Guid?>(player.Guid);        
        }

        private Entities.Player MapPlayer(UpdatePlayerRequest request) =>
        new Entities.Player(request.SquadId, request.PlayerId) {  
            DateOfBirth = request.DateOfBirth.Value,
            DominantFoot = request.DominantFoot,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Nationality = request.Nationality,
            Positions = request.Positions,
            SquadNumber = request.SquadNumber
        };        
    }
}