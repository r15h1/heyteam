using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using System.Linq;
using System;
using System.Collections.Generic;

namespace HeyTeam.Core.UseCases.Squad {
    public class GetSquadUseCase : IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>>
    {
        private readonly IClubRepository clubRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IPlayerRepository playerRepository;
        private readonly IValidator<GetSquadRequest> validator;
        
        public GetSquadUseCase(IClubRepository clubRepository, ISquadRepository squadRepository, IPlayerRepository playerRepository,IValidator<GetSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.playerRepository = playerRepository;
            this.validator = validator;
        }
        public Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>> Execute(GetSquadRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>.CreateResponse(validationResult.Messages);

            var club = clubRepository.GetClub(request.ClubId);
            if (club == null)
                return Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>.CreateResponse(new EntityNotFoundException());
            
            var squad = squadRepository.GetSquad(request.SquadId);
            if (squad == null)
                return Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>.CreateResponse(new EntityNotFoundException());

            var players = playerRepository.GetPlayers(request.SquadId);

            var result = new System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>(squad, players);
            var response = new Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>(result);
            return response;
        }
    }
}