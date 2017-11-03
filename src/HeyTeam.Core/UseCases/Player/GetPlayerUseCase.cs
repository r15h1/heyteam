using HeyTeam.Core.Exceptions;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.Validation;
using HeyTeam.Util;
using System;

namespace HeyTeam.Core.UseCases.Player {
	public class GetPlayerUseCase : IUseCase<GetPlayerRequest, Response<(Entities.Player, string)>>	{
		private readonly ISquadRepository squadRepository;
		private readonly IPlayerRepository playerRepository;
		private readonly IValidator<GetPlayerRequest> validator;

		public GetPlayerUseCase(ISquadRepository squadRepository, IPlayerRepository playerRepository, IValidator<GetPlayerRequest> validator) {
			Ensure.ArgumentNotNull(squadRepository);
			Ensure.ArgumentNotNull(playerRepository);
			Ensure.ArgumentNotNull(validator);
			this.squadRepository = squadRepository;
			this.playerRepository = playerRepository;
			this.validator = validator;
		}
		public Response<(Entities.Player, string)> Execute(GetPlayerRequest request) {
			if (request == null || request.PlayerId.IsEmpty())
				return new Response<(Entities.Player, string)>(new ArgumentNullException());

			var player = playerRepository.GetPlayer(request.PlayerId);
			if (player == null)
				return new Response<(Entities.Player, string)>(new EntityNotFoundException("The player could not be found"));

			var squad = squadRepository.GetSquad(player.SquadId);
			if(squad == null)
				return new Response<(Entities.Player, string)>(new EntityNotFoundException("The squad for this player could not be found"));

			return new Response<(Entities.Player, string)>((player, squad.Name));
		}
	}
}
