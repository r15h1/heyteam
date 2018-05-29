using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HeyTeam.Web.Areas.Players.Controllers {

	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class SquadsController : Controller {
        private readonly Club club;
        private readonly ISquadService squadService;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;

		public SquadsController(
                Club club,
				ISquadService squadService,
				ISquadQuery squadQuery,
				IMemberQuery memberQuery
		) {
            this.club = club;
            this.squadService = squadService;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
		}

        [HttpGet("{squadId:guid}")]
        public IActionResult Index([FromRoute]string squadId) {
            ViewData["Title"] = "Squad Details";
			var response = squadQuery.GetFullSquadDetails(System.Guid.Parse(squadId));
            var model = new SquadDetailsViewModel { SquadName = response.Squad.Name, SquadId = response.Squad.Guid.ToString(), Players = response.Players, Coach = response.Coach };
            return View("Index", model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }		
	}
}