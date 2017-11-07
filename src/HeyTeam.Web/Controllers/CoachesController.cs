using HeyTeam.Core.Entities;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Web.Models.CoachViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Route("[controller]")]
	public class CoachesController : Controller {
		private readonly Club club;
		private readonly IUseCase<SaveCoachRequest, Response<Guid?>> saveCoachUseCase;
		private readonly IUseCase<GetCoachListRequest, Response<IEnumerable<Coach>>> getCoachListUseCase;

		public CoachesController(Club club, 
			IUseCase<SaveCoachRequest, Response<Guid?>> saveCoachUseCase,
			IUseCase<GetCoachListRequest, Response<IEnumerable<Coach>>> getCoachListUseCase
		) {
			this.club = club;	
			this.saveCoachUseCase = saveCoachUseCase;
			this.getCoachListUseCase = getCoachListUseCase;
		}

		[HttpGet]
		public IActionResult Index() {
			List<CoachViewModel> coaches = new List<CoachViewModel>();
			var response = getCoachListUseCase.Execute(new GetCoachListRequest { ClubId = club.Guid });
			foreach (var coach in response.Result)
				coaches.Add(Map(coach));
				
			return View(coaches);			
		}

		[HttpGet("new")]
        public IActionResult Create() {
            return View();
        }

		[HttpPost("new")]
		public IActionResult Create(CoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = saveCoachUseCase.Execute(Map(model, SaveCoachRequest.Action.ADD));
			if (!response.WasRequestFulfilled) { 
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction("Index");
		}

		[HttpGet("{coachId:guid}")]
		public IActionResult Edit(string coachId) {
			var response = getCoachListUseCase.Execute(new GetCoachListRequest { ClubId = club.Guid });
			var coach = response.Result.FirstOrDefault(c => c.Guid.ToString().Equals(coachId));
			return View(Map(coach));
		}

		[HttpPost("{coachId:guid}")]
		public IActionResult Edit(CoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = saveCoachUseCase.Execute(Map(model, SaveCoachRequest.Action.UPDATE));
			if (!response.WasRequestFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction("Index");
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

		private SaveCoachRequest Map(CoachViewModel model, SaveCoachRequest.Action command) => new SaveCoachRequest {
				CoachId = model.CoachId,
				ClubId = club.Guid,
				DateOfBirth = model.DateOfBirth,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Phone = model.Phone,
				Qualifications = model.Qualifications,
				Command = command
			};
	
	}
}