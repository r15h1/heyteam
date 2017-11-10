using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Entities;
using System.Collections.Generic;
using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Web.Models.CoachViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeyTeam.Web.Controllers {
    
    [Authorize]
    [Route("[controller]")]
    public class SquadsController : Controller {
        private readonly Club club;
        private readonly IUseCase<AddSquadRequest, Response<Guid?>> addSquadUseCase;
        private readonly IUseCase<GetSquadRequest, Response<(Squad, IEnumerable<Player>, Coach)>> getSquadUseCase;
		private readonly IUseCase<GetCoachListRequest, Response<IEnumerable<Coach>>> getCoachListUseCase;
		private readonly IUseCase<SquadCoachChangeRequest, Response<string>> squadCoachChangeInteractor;

		public SquadsController(
                Club club, 
                IUseCase<AddSquadRequest, Response<Guid?>> addSquadUseCase,
                IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>, Core.Entities.Coach>>> getSquadUseCase,
				IUseCase<GetCoachListRequest, Response<IEnumerable<Core.Entities.Coach>>> getCoachListUseCase,
				IUseCase<SquadCoachChangeRequest, Response<string>> squadCoachChangeInteractor
		) {
            this.club = club;
            this.addSquadUseCase = addSquadUseCase;
            this.getSquadUseCase = getSquadUseCase;
			this.getCoachListUseCase = getCoachListUseCase;
			this.squadCoachChangeInteractor = squadCoachChangeInteractor;
		}

        [HttpGet("{squadId:guid}")]
        public IActionResult Index([FromRoute]string squadId) {
            ViewData["Title"] = "Squad Details";
            var request = new GetSquadRequest { ClubId = club.Guid, SquadId = System.Guid.Parse(squadId) };
            var response = getSquadUseCase.Execute(request);
            var model = new SquadDetailsViewModel { SquadName = response.Result.Item1.Name, SquadId = response.Result.Item1.Guid.ToString(), Players = response.Result.Item2, Coach = response.Result.Item3 };
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
            

            var result = addSquadUseCase.Execute(new AddSquadRequest{
                SquadName = squad.SquadName,
                ClubId = club.Guid
            });
            
            if(result.WasRequestFulfilled) {
                return RedirectToLocal("/");
            } else {
                foreach(var error in result.Errors)
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
			var squadRequest = new GetSquadRequest { ClubId = club.Guid, SquadId = System.Guid.Parse(squadId) };
			var squadResponse = getSquadUseCase.Execute(squadRequest);
			var coachRequest = new GetCoachListRequest { ClubId = club.Guid };
			var coachResponse = getCoachListUseCase.Execute(coachRequest);
			var model = new AssignCoachViewModel { 
				Coaches = coachResponse.Result.Select(c => new SelectListItem { Text = $"{c.FirstName} {c.LastName}", Value = c.Guid.ToString()}).OrderBy(c => c.Text).ToList(),
				SquadId = squadResponse.Result.Item1.Guid,
				SquadName = squadResponse.Result.Item1.Name
			};

			return View(model);
		}

		[HttpPost("{squadId:guid}/coach")]
		public IActionResult Coach(AssignCoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var request = new SquadCoachChangeRequest { 
				CoachId = model.SelectedCoach.Value, 
				SquadId = model.SquadId, 
				Operation = SquadCoachChangeRequest.SquadCoachOperation.ADD
			};

			var response = squadCoachChangeInteractor.Execute(request);
			if (!response.WasRequestFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction("Index");
		}
	}
}