using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Services {
	public class AvailabilityService : IAvailabilityService {
		private readonly IValidator<NewAvailabilityRequest> newAvailabilityValidator;
		private readonly IAvailabilityRepository repository;
		private readonly IClubQuery clubQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;
        private readonly IAvailabilityQuery availabilityQuery;

        public AvailabilityService(IValidator<NewAvailabilityRequest> newAvailabilityValidator, IAvailabilityRepository repository, 
            IClubQuery clubQuery, ISquadQuery squadQuery, IMemberQuery memberQuery, IAvailabilityQuery availabilityQuery
            ) {
			this.newAvailabilityValidator = newAvailabilityValidator;
			this.repository = repository;
			this.clubQuery = clubQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
            this.availabilityQuery = availabilityQuery;
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

        public Response DeleteAvailability(DeleteAvailabilityRequest request)
        {
            if (request == null) return Response.CreateResponse(new ArgumentNullException());

			var response = VerifyIntegrity(request.ClubId, request.PlayerId, request.AvailabilityId);
			if (response != null) return response;

			try {
                repository.DeleteAvailability(request);
                return Response.CreateSuccessResponse();
            }catch(Exception ex) {
                return Response.CreateResponse(ex);
            }
        }

		public Response UpdateAvailability(UpdateAvailabilityRequest request) {
			if (request == null) return Response.CreateResponse(new ArgumentNullException());

			var response = VerifyIntegrity(request.ClubId, request.PlayerId, request.AvailabilityId);
			if (response != null) return response;

			try {
				repository.UpdateAvailability(request);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		private Response VerifyIntegrity(Guid clubId, Guid playerId, Guid availabilityId) {
			if (clubId.IsEmpty() || playerId.IsEmpty() || availabilityId.IsEmpty())
				return Response.CreateResponse(new ArgumentNullException("ClubId, playerId and AvailabilityId cannot be empty"));

			var club = clubQuery.GetClub(clubId);
			if (club == null)
				Response.CreateResponse(new EntityNotFoundException("The specified club does not exist"));

			var player = memberQuery.GetPlayer(playerId);
			if (player == null)
				Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));

			var squad = squadQuery.GetSquad(player.SquadId);
			if (squad == null || squad.ClubId != club.Guid)
				return Response.CreateResponse(new IllegalOperationException("The specified player does not belong to this club"));

			var availability = availabilityQuery.GetAvailability(clubId, availabilityId);
			if (availability == null)
				Response.CreateResponse(new EntityNotFoundException("The specified availability does not exist"));

			if (availability.PlayerId != playerId)
				return Response.CreateResponse(new IllegalOperationException("This availability does not belong to the specified player"));

			return null;
		}
	}
}
