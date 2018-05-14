using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.PlayerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers {

    [Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/squads/{squadId:guid}/[controller]")]
    public class PlayersController : Controller {
		private readonly Club club;
		private readonly IPlayerService playerService;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery playerQuery;

		public PlayersController(
			Club club,
			ISquadQuery squadQuery,
			IPlayerService playerService,
			IMemberQuery playerQuery
		) {
			this.club = club;
			this.squadQuery = squadQuery;
			this.playerService = playerService;
			this.playerQuery = playerQuery;
		}

		[Route("new")]
		[HttpGet]
		public IActionResult New(string squadId) {
			SetupTitle(squadId, "Add New Player");
			return View("Create");
		}

		private void SetupTitle(string squadId, string title) {
			var squad = squadQuery.GetSquad(System.Guid.Parse(squadId));
			ViewData["SquadId"] = squad.Guid;
			ViewData["Title"] = title;
			ViewData["SubTitle"] = squad.Name;
		}

		[Route("new")]
		[HttpPost]
		public IActionResult New(PlayerViewModel player) {
			if (!ModelState.IsValid) {
				SetupTitle(player.SquadId.ToString(), "Add New Player");
				return View("Create", player);
			}

			PlayerRequest request = MapPlayerRequest(player);
			var response = playerService.RegisterPlayer(request);
			if (!response.RequestIsFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View("Create", player);
			}

			return RedirectToAction("Index", "Squads", new { SquadId = player.SquadId });
		}

		private PlayerRequest MapPlayerRequest(PlayerViewModel player) {
			return new PlayerRequest {
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

		[Route("{playerId:guid}")]
		[HttpGet]
		public IActionResult Edit(string squadId, string playerId) {
			var player = playerQuery.GetPlayer(System.Guid.Parse(playerId));
			if(player == null)
				return View("PlayerNotFound", squadId);

			var squad = squadQuery.GetSquad(player.SquadId);
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
				SquadName = squad.Name
			};

			return View("Edit", model);
		}

		[Route("{playerId:guid}")]
		[HttpPost]
		public IActionResult Edit(PlayerViewModel player) {
			if (!ModelState.IsValid)
				return View("Edit", player);


			var request = new PlayerRequest {
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
			var response = playerService.UpdatePlayerProfile(request);
			if (!response.RequestIsFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View("Edit", player);
			}

            return RedirectToAction("Index", "Squads", new { SquadId = player.SquadId });
        }
	}
}