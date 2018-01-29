using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.AvailabilityViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
	[Area("Administration")]
	[Route("[area]/[controller]")]
	public class AvailabilityController : Controller
    {
		private readonly Club club;
		private readonly IAvailabilityQuery availabilityQuery;
		private readonly IAvailabilityService availabilityService;

		public AvailabilityController(Club club, IAvailabilityQuery availabilityQuery, IAvailabilityService availabilityService){
			this.club = club;
			this.availabilityQuery = availabilityQuery;
			this.availabilityService = availabilityService;
		}

		[HttpGet("")]
        public IActionResult Index() {
			var availabilities = availabilityQuery.GetAvailabilities(new GetAvailabilityRequest { ClubId = club.Guid });
            return View(availabilities);
        }

		[HttpGet("new")]
		public IActionResult New() {
			return View();
		}

		[HttpPost("new")]
		public IActionResult New(NewAvailabilityViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var request = new NewAvailabilityRequest { 
				AvailabilityStatus = model.AvailabilityStatus.Value,
				ClubId = club.Guid,
				DateFrom = model.DateFrom.Value,
				DateTo = model.DateTo,
				Notes = model.Notes,
				PlayerId = model.PlayerId
			};

			var response = availabilityService.AddAvailability(request);
			if(!response.RequestIsFulfilled){
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}				

			return RedirectToAction(nameof(Index));
		}
	}
}