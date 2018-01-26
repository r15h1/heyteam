using HeyTeam.Core;
using HeyTeam.Core.Queries;
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

		public AvailabilityController(Club club, IAvailabilityQuery availabilityQuery){
			this.club = club;
			this.availabilityQuery = availabilityQuery;
		}

		[HttpGet("")]
        public IActionResult Index() {
			var availabilities = availabilityQuery.GetAvailabilities(new AvailabilityRequest { ClubId = club.Guid });
            return View(availabilities);
        }

		[HttpGet("new")]
		public IActionResult New() {
			return View();
		}
	}
}