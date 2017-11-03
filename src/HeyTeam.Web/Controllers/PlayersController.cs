using HeyTeam.Core.Entities;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Player;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Web.Models.PlayerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers
{

	[Authorize]    
    public class PlayersController : Controller {
		private readonly Club club;
		
		private readonly IUseCase<GetSquadRequest, Response<(Squad, IEnumerable<Player>)>> getSquadUseCase;
		private readonly IUseCase<AddPlayerRequest, Response<Guid?>> addPlayerUseCase;
		private readonly IUseCase<UpdatePlayerRequest, Response<Guid?>> updatePlayerUseCase;
		private readonly IUseCase<GetPlayerRequest, Response<(Player, string)>> getPlayerUseCase;

		public PlayersController(
			Club club,
			IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>> getSquadUseCase,
			IUseCase<AddPlayerRequest, Response<Guid?>> addPlayerUseCase,
			IUseCase<UpdatePlayerRequest, Response<Guid?>> updatePlayerUseCase,
			IUseCase<GetPlayerRequest, Response<(Core.Entities.Player, string)>> getPlayerUseCase
		) {
			this.club = club;
            this.getPlayerUseCase = getPlayerUseCase;
			this.getSquadUseCase = getSquadUseCase;
			this.addPlayerUseCase = addPlayerUseCase;
			this.updatePlayerUseCase = updatePlayerUseCase;
		}

        [Route("squads/{squadId:guid}/[controller]/new")]
        [HttpGet]
        public IActionResult New(string squadId) {
			SetupTitle(squadId, "Add New Player");
			return View("Create");
		}

		private void SetupTitle(string squadId, string title) {
			var request = new GetSquadRequest { ClubId = club.Guid, SquadId = System.Guid.Parse(squadId) };
			var response = getSquadUseCase.Execute(request);
			ViewData["SquadId"] = squadId;
			ViewData["Title"] = title;
			ViewData["SubTitle"] = response.Result.Item1.Name;
		}

		[Route("squads/{squadId:guid}/[controller]/new")]
		[HttpPost]
		public IActionResult New(PlayerViewModel player) {
			if (!ModelState.IsValid) {
				SetupTitle(player.SquadId.ToString(), "Add New Player");
				return View("Create", player);
			}

			AddPlayerRequest request = MapAddPlayerRequest(player);
			var response = addPlayerUseCase.Execute(request);
			if(!response.WasRequestFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View("Create", player);
			}				
			
			return Redirect($"/squads/{player.SquadId}");
		}

		private AddPlayerRequest MapAddPlayerRequest(PlayerViewModel player) {
			return new AddPlayerRequest {
				DateOfBirth = player.DateOfBirth,
				DominantFoot = player.DominantFoot.FirstOrDefault(),
				Email = player.Email,
				FirstName = player.FirstName,
				LastName = player.LastName,
				Nationality = player.Nationality,
				SquadNumber = player.SquadNumber,
				SquadId = player.SquadId
			};
		}

		[Route("players/{playerId:guid}")]
        [HttpGet]
        public IActionResult Edit(string squadId, string playerId) {			
			var response = getPlayerUseCase.Execute(new GetPlayerRequest { PlayerId = System.Guid.Parse(playerId) });
			if(response.WasRequestFulfilled)
			{
				var player = response.Result.Item1;
				var model = new PlayerViewModel {					
					DateOfBirth = player.DateOfBirth,
					DominantFoot = player.DominantFoot.ToString(),
					Email = player.Email,
					FirstName = player.FirstName,
					LastName = player.LastName,
					Nationality = player.Nationality,
					SquadNumber = player.SquadNumber,
					SquadId = player.SquadId,
					PlayerId = player.Guid,
					SquadName = response.Result.Item2
				};

				return View("Edit", model);
			}
            return View("Details");
		}

		[Route("players/{playerId:guid}")]
		[HttpPost]
		public IActionResult Edit(PlayerViewModel player) {
			if (!ModelState.IsValid)
				return View("Edit", player);


			var request = new UpdatePlayerRequest
			{
				DateOfBirth = player.DateOfBirth,
				DominantFoot = player.DominantFoot.FirstOrDefault(),
				Email = player.Email,
				FirstName = player.FirstName,
				LastName = player.LastName,
				Nationality = player.Nationality,
				SquadNumber = player.SquadNumber,
				SquadId = player.SquadId,
				PlayerId = player.PlayerId.Value
			};
			var response = updatePlayerUseCase.Execute(request);
			if (!response.WasRequestFulfilled)
			{
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View("Edit", player);
			}

			return Redirect($"/squads/{player.SquadId}");
		}
	}        
}