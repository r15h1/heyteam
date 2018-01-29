using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using System;

namespace HeyTeam.Lib.Services {
	public class AvailabilityService : IAvailabilityService {
		private readonly IValidator<NewAvailabilityRequest> newAvailabilityValidator;
		private readonly IAvailabilityRepository repository;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;

		public AvailabilityService(IValidator<NewAvailabilityRequest> newAvailabilityValidator, IAvailabilityRepository repository, IClubQuery clubQuery, ISquadQuery squadQuery, IMemberQuery memberQuery) {
			this.newAvailabilityValidator = newAvailabilityValidator;
			this.repository = repository;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
		}

		public Response AddAvailability(NewAvailabilityRequest request) {
			var validationResult = newAvailabilityValidator.Validate(request);
			if(!validationResult.IsValid) 
				return Response.CreateResponse(validationResult.Messages);

			var club = clubQuery.GetClub(request.ClubId);
			if (club == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var player = memberQuery.GetPlayer(request.PlayerId);
			if(player == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));

			var squad = squadQuery.GetSquad(player.SquadId);
			if(squad == null || squad.ClubId != club.Guid)
				return Response.CreateResponse(new IllegalOperationException("The specified player does not belong to this club"));

			try {
				repository.AddAvailability(request);
				return Response.CreateSuccessResponse();
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
		}
	}
}
