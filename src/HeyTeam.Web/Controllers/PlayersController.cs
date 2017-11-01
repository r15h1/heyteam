using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeyTeam.Core.Repositories;
using HeyTeam.Web.Models.PlayerViewModels;

namespace HeyTeam.Web.Controllers
{

    [Authorize]
    [Route("squads/{squadId:guid}/[controller]")]
    public class PlayersController : Controller {
        private readonly IPlayerRepository playerRepository;

        public PlayersController(IPlayerRepository playerRepository) {
            this.playerRepository = playerRepository;
        }

        [HttpGet("new")]
        public IActionResult New() {
            ViewData["Title"] = "Add New Player";
            return View("Edit");
        }

        [HttpGet("{playerId:guid}")]
        public IActionResult Details(string squadId, string playerId) {
            var player = playerRepository.GetPlayer(System.Guid.Parse(playerId));
            PlayerDetailsViewModel model = new PlayerDetailsViewModel {
                Name = $"{player.FirstName} {player.LastName}"
            };
            ViewData["Title"] = "Player Details";
            return View("Details", model);
        }



    }        
}