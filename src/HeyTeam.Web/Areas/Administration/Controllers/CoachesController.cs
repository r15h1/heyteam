using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Util;
using HeyTeam.Web.Models.CoachViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers
{
    [Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/[controller]")]
    public class CoachesController : Controller {
		private readonly Club club;
		private readonly ICoachService coachService;
		private readonly IMemberQuery memberQuery;

		public CoachesController(Club club,
			ICoachService coachService,
			IMemberQuery memberQuery
		) {
			this.club = club;	
			this.coachService = coachService;
			this.memberQuery = memberQuery;
		}

		[HttpGet]
		public IActionResult Index() {
			List<CoachViewModel> list = new List<CoachViewModel>();
			var coaches = memberQuery.GetClubCoaches(club.Guid);
			foreach (var coach in coaches)
				list.Add(Map(coach));
				
			return View(list);			
		}

		[HttpGet("new")]
        public IActionResult Create(string returnurl) {
			ViewData["ReturnUrl"] = returnurl ?? "/administration/coaches";
            return View();
        }

		[HttpPost("new")]
		public IActionResult Create(CoachViewModel model, string returnurl) {
			ViewData["ReturnUrl"] = returnurl ?? "/administration/coaches";

			if (!ModelState.IsValid)
				return View(model);

			var response = coachService.RegisterCoach(Map(model));
			if (!response.RequestIsFulfilled) {
				UpdateModelErrors(response.Errors);
				return View(model);
			}

			return ActionResultOnSuccess(returnurl);
		}

		public IActionResult ActionResultOnSuccess(string returnUrl = null) {
			if (returnUrl.IsEmpty())
				return RedirectToAction("Index");

			return Redirect(returnUrl);
		}

		public void UpdateModelErrors(IEnumerable<string> errors) {
			foreach (var error in errors)
				ModelState.AddModelError("", error);
		}

		[HttpGet("{coachId:guid}")]
		public IActionResult Edit(string coachId) {
			var coaches = memberQuery.GetClubCoaches(club.Guid);
			var coach = coaches.FirstOrDefault(c => c.Guid.ToString().Equals(coachId));
			return View(Map(coach));
		}

		[HttpPost("{coachId:guid}")]
		public IActionResult Edit(CoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = coachService.UpdateCoachProfile(Map(model));
			if (!response.RequestIsFulfilled) {
				UpdateModelErrors(response.Errors);
				return View(model);
			}

			return ActionResultOnSuccess();
		}

		private CoachViewModel Map(Coach coach) => new CoachViewModel {
				ClubId = coach.ClubId,
				CoachId = coach.Guid,
				DateOfBirth = coach.DateOfBirth,
				Email = coach.Email,
				FirstName = coach.FirstName,
				LastName = coach.LastName,
				Phone = coach.Phone,
				Qualifications = coach.Qualifications				
			};

		private CoachRequest Map(CoachViewModel model) => new CoachRequest {
				CoachId = model.CoachId,
				ClubId = club.Guid,
				DateOfBirth = model.DateOfBirth,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Phone = model.Phone,
				Qualifications = model.Qualifications
			};
	
	}
}