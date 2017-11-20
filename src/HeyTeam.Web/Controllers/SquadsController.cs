using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.CoachViewModels;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers {

	[Authorize]
    [Route("[controller]")]
    public class SquadsController : Controller {
        private readonly Club club;
        private readonly ISquadService squadService;
		private readonly ISquadQuery squadQuery;
		private readonly ICoachQuery coachQuery;

		public SquadsController(
                Club club,
				ISquadService squadService,
				ISquadQuery squadQuery,
				ICoachQuery coachQuery
		) {
            this.club = club;
            this.squadService = squadService;
			this.squadQuery = squadQuery;
			this.coachQuery = coachQuery;
		}

        [HttpGet("{squadId:guid}")]
        public IActionResult Index([FromRoute]string squadId) {
            ViewData["Title"] = "Squad Details";
			var response = squadQuery.GetFullSquadDetails(System.Guid.Parse(squadId));
            var model = new SquadDetailsViewModel { SquadName = response.Squad.Name, SquadId = response.Squad.Guid.ToString(), Players = response.Players, Coach = response.Coach };
            return View("Index", model);
        }

        [HttpGet("new")]
        public IActionResult New() {
            ViewData["Title"] = "New Squad";
            ViewData["ReturnUrl"] = "/";
            return View("Edit");
        }

        [HttpPost("new")]
        public IActionResult New([FromForm]SquadViewModel squad) {
            ViewData["Title"] = "New Squad";
            ViewData["ReturnUrl"] = "/";

            if (!ModelState.IsValid)
                return View("Edit", squad);
            

            var response = squadService.RegisterSquad(new SquadRequest{
                SquadName = squad.SquadName,
                ClubId = club.Guid
            });
            
            if(response.RequestIsFulfilled) {
                return RedirectToLocal("/");
            } else {
                foreach(var error in response.Errors)
                    ModelState.AddModelError("", error);                
                
                return View("Edit", squad);
            }            
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

		[HttpGet("{squadId:guid}/coach")]
		public IActionResult Coach([FromRoute]string squadId) {
			var response = squadQuery.GetFullSquadDetails(System.Guid.Parse(squadId));
			var coaches = coachQuery.GetClubCoaches(club.Guid);
			if (response.Coach != null)
				coaches = coaches.Where(c => !c.Guid.Equals(response.Coach.Guid));

			var model = new AssignCoachViewModel { 
				Coaches = coaches.Select(c => new SelectListItem { Text = $"{c.FirstName} {c.LastName}", Value = c.Guid.ToString()}).OrderBy(c => c.Text).ToList(),
				SquadId = response.Squad.Guid,
				SquadName = response.Squad.Name
			};

			return View(model);
		}

		[HttpPost("{squadId:guid}/coach")]
		public IActionResult Coach(AssignCoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = squadService.AssignCoach(model.SquadId, model.SelectedCoach.Value);
			if(!response.RequestIsFulfilled) {
				AddModelErrors(response.Errors);
				return View(model);
			}

			return RedirectToAction("Index");			
		}

		[HttpPost("{squadId:guid}")]
		public IActionResult CoachUnassignment(CoachUnAssignmentViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = squadService.UnAssignCoach(model.SquadId.Value, model.CoachId.Value);
			if (!response.RequestIsFulfilled) {
				AddModelErrors(response.Errors);
				return View(model);
			}

			return RedirectToAction("Index");
		}

		private void AddModelErrors(IEnumerable<string> errors) {
			foreach (var error in errors)
				ModelState.AddModelError("", error);
		}
	}
}