using System;
using System.Linq;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using HeyTeam.Util;

namespace HeyTeam.Core.UseCases.Squad {
    public class UpdateSquadUseCase : IUseCase<UpdateSquadRequest, Response<Guid?>> {
        private IClubRepository clubRepository;
        private ISquadRepository squadRepository;
        private IValidator<UpdateSquadRequest> validator;

        public UpdateSquadUseCase(IClubRepository clubRepository, ISquadRepository squadRepository, IValidator<UpdateSquadRequest> validator) {
            this.clubRepository = clubRepository;
            this.squadRepository = squadRepository;
            this.validator = validator;
        }

        public Response<Guid?> Execute(UpdateSquadRequest request) {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return Response<Guid?>.CreateResponse(validationResult.Messages);

            var club = clubRepository.Get(request.ClubId);
            if(club == null)
                return Response<Guid?>.CreateResponse(new ClubNotFoundException());

            var squad = new Entities.Squad(club, request.SquadId) { Name = request.SquadName };       

            try {
                club.UpdateSquad(squad);
                squadRepository.Update(squad);
            } catch (Exception ex) { 
                return Response<Guid?>.CreateResponse(ex); 
            }    

            return new Response<Guid?>(squad.Guid);
        }
    }
}