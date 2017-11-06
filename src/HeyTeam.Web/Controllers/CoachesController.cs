using HeyTeam.Core.Entities;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Coach;
using HeyTeam.Web.Models.CoachViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Route("[controller]")]
	public class CoachesController : Controller {
		private readonly Club club;
		private readonly IUseCase<AddCoachRequest, Response<Guid?>> addCoachUseCase;

		public CoachesController(Club club, IUseCase<AddCoachRequest, Response<Guid?>> addCoachUseCase) {
			this.club = club;	
			this.addCoachUseCase = addCoachUseCase;
		}
		
		[HttpGet("new")]
        public IActionResult Create() {
            return View();
        }

		[HttpPost("new")]
		public IActionResult Create(CoachViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var response = addCoachUseCase.Execute(Map(model));
			if (!response.WasRequestFulfilled) { 
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return Redirect("/"); ;
		}

		private AddCoachRequest Map(CoachViewModel model) => new AddCoachRequest {
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