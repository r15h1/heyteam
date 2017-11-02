using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Web.Models.PlayerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using HeyTeam.Core.Entities;
using System;
using HeyTeam.Core.UseCases.Player;
using System.Linq;

namespace HeyTeam.Web.Controllers
{

	[Authorize]    
    public class PlayersController : Controller {
		private readonly Club club;
		private readonly IPlayerRepository playerRepository;
		private readonly IUseCase<GetSquadRequest, Response<(Squad, IEnumerable<Player>)>> getSquadUseCase;
		private readonly IUseCase<AddPlayerRequest, Response<Guid?>> addPlayerUseCase;

		public PlayersController(
			Club club,
			IPlayerRepository playerRepository,
			IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>> getSquadUseCase,
			IUseCase<AddPlayerRequest, Response<Guid?>> addPlayerUseCase
		) {
			this.club = club;
            this.playerRepository = playerRepository;
			this.getSquadUseCase = getSquadUseCase;
			this.addPlayerUseCase = addPlayerUseCase;
		}

        [Route("squads/{squadId:guid}/[controller]/new")]
        [HttpGet]
        public IActionResult New(string squadId)
		{
			SetupTitle(squadId);
			return View("Edit");
		}

		private void SetupTitle(string squadId)
		{
			var request = new GetSquadRequest { ClubId = club.Guid, SquadId = System.Guid.Parse(squadId) };
			var response = getSquadUseCase.Execute(request);
			ViewData["SquadId"] = squadId;
			ViewData["Title"] = $"{response.Result.Item1.Name} - Add New Player";
		}

		[Route("squads/{squadId:guid}/[controller]/new")]
		[HttpPost]
		public IActionResult New(PlayerDetailsViewModel player)
		{
			if (!ModelState.IsValid) {
				SetupTitle(player.SquadId.ToString());
				return View("Edit", player);
			}

			var request = new AddPlayerRequest
			{
				DateOfBirth = player.DateOfBirth,
				DominantFoot = player.DominantFoot.FirstOrDefault(),
				Email = player.Email,
				FirstName = player.FirstName,
				LastName = player.LastName,
				Nationality = player.Nationality,
				SquadNumber = player.SquadNumber,
				SquadId = player.SquadId
			};
			var response = addPlayerUseCase.Execute(request);
			return Redirect($"/squads/{player.SquadId}");
		}

		[Route("players/{playerId:guid}")]
        [HttpGet]
        public IActionResult Details(string squadId, string playerId) {			
			var player = playerRepository.GetPlayer(System.Guid.Parse(playerId));
            PlayerDetailsViewModel model = new PlayerDetailsViewModel {
                FirstName = $"{player.FirstName} {player.LastName}"
            };
            ViewData["Title"] = "Player Details";			
			return View("Details", model);
        }
    }        
}