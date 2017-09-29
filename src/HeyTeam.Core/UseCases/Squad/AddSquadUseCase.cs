using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Entities;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using System.Linq;
using System;

namespace HeyTeam.Core.UseCases.Squad {
    public class AddSquadUseCase : IUseCase<AddSquadRequest, Response<Guid?>>
    {
        private readonly IClubRepository clubRepository;
        private readonly ISquadRepository squadRepository;
        private readonly IValidator<AddSquadRequest> validator;

        public AddSquadUseCase(IClubRepository clubRepository, ISquadRepository squadRepository, IValidator<AddSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.validator = validator;
        }
        public Response<Guid?> Execute(AddSquadRequest request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<Guid?>.CreateResponse(validationResult.Messages);

            var club = clubRepository.Get(request.ClubId);
            if (club == null)
                return Response<Guid?>.CreateResponse(new ClubNotFoundException());

            var squad = new Entities.Squad(club.Guid) { Name = request.SquadName };
            
            try {
                club.AddSquad(squad);
                squadRepository.Add(squad);
            } catch (Exception ex) {
                return Response<Guid?>.CreateResponse(ex);
            }        

            return new Response<Guid?>(squad.Guid);
        }
    }
}