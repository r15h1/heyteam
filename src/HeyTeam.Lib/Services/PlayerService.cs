using HeyTeam.Core;
using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Identity;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Services;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;
using System.Linq;

namespace HeyTeam.Lib.Services {
	public class PlayerService : IPlayerService {
		private readonly IPlayerRepository playerRepository;
		private readonly IMemberQuery memberQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IValidator<PlayerRequest> validator;
		private readonly IIdentityManager identityManager;

		public PlayerService(IPlayerRepository playerRepository, 
			IMemberQuery playerQuery, ISquadQuery squadQuery, IValidator<PlayerRequest> validator,
			IIdentityManager identityManager
		)	{
			this.playerRepository = playerRepository;
			this.memberQuery = playerQuery;
			this.squadQuery = squadQuery;
			this.validator = validator;
			this.identityManager = identityManager;
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
				return Response.CreateSuccessResponse();
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

			var player = memberQuery.GetPlayer(request.PlayerId.Value);

			if (player == null || player.SquadId != request.SquadId)
				return Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));

			player = MapPlayer(request);

			try {
				playerRepository.UpdatePlayer(player);
				return Response.CreateSuccessResponse();
			} catch (Exception ex) {
				return Response.CreateResponse(ex);
			}
		}

		public Response DeletePlayer(DeletePlayerRequest request) {
			var player = memberQuery.GetPlayer(request.PlayerId);

			if (player == null || player.SquadId != request.SquadId)
				return Response.CreateResponse(new EntityNotFoundException("The specified player does not exist"));

			var squad = squadQuery.GetSquad(request.SquadId);
			if(squad == null)
				return Response.CreateResponse(new EntityNotFoundException("The specified squad does not exist"));
			else if (squad.ClubId != request.ClubId)
				return Response.CreateResponse(new IllegalOperationException("The specified squad does not belong to this club"));

			try {
				playerRepository.DeletePlayer(player);
				var members = memberQuery.GetMembersByEmail(request.ClubId, player.Email);
				if(members.Count() == 0){
					var result = identityManager.RemoveUser(player.Email).Result;
				} else if(!members.Any(m => m.Membership == Membership.Player)){
					var result = identityManager.RemoveUserRole(player.Email, Membership.Player).Result;
				}

				return Response.CreateSuccessResponse();
			} catch(Exception ex){
				return Response.CreateResponse(ex);
			}
		}
	}
}