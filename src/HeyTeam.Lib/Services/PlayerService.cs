using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Lib.Services {
	public class PlayerService : IPlayerService {
		private readonly IPlayerRepository playerRepository;
		private readonly IPlayerQuery playerQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IValidator<PlayerRequest> validator;

		public PlayerService(IPlayerRepository playerRepository, IPlayerQuery playerQuery, ISquadQuery squadQuery, IValidator<PlayerRequest> validator)	{
			this.playerRepository = playerRepository;
			this.playerQuery = playerQuery;
			this.squadQuery = squadQuery;
			this.validator = validator;
		}

		public Response RegisterPlayer(PlayerRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);

			var squad = squadQuery.GetSquad(request.SquadId);
			if (squad == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified squad does not exist"));

			Player player = MapPlayer(request);
			try {
				playerRepository.AddPlayer(player);
				return Response.CreateResponse();
			} catch(Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		private Player MapPlayer(PlayerRequest request) =>
			 new Player(request.SquadId, request.PlayerId) {
				DateOfBirth = request.DateOfBirth.Value,
				DominantFoot = request.DominantFoot,
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Nationality = request.Nationality,
				Positions = request.Positions,
				SquadNumber = request.SquadNumber
			};
	
		public Response UpdatePlayerProfile(PlayerRequest request) {
			var validationResult = validator.Validate(request);
			if (!validationResult.IsValid)
				return Response.CreateResponse(validationResult.Messages);
			
			if(request.PlayerId.IsEmpty())
				return Response.CreateResponse(new ArgumentNullException("PlayerId cannot be null"));

			var player = playerQuery.GetPlayer(request.PlayerId.Value);

			if (player == null || player.SquadId != request.SquadId)
				return Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));

			player = MapPlayer(request);

			try {
				playerRepository.UpdatePlayer(player);
				return Response.CreateResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}
	}
}