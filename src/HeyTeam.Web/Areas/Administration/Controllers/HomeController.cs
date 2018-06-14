using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using HeyTeam.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
	[Area("Administration")]
	[Route("[area]/[controller]")]
	public class HomeController : Controller {
		private readonly Club club;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IMemberQuery memberQuery;
        private readonly ILogger<HomeController> logger;

        public HomeController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery, ILogger<HomeController> logger) {
			this.club = club;
			this.userManager = userManager;
			this.memberQuery = memberQuery;
            this.logger = logger;
        }

		[HttpGet]
		public IActionResult Index(Guid memberid) {
			var model = new DashboardModel {
				MemberId = memberid,
				Membership = Membership.Coach
			};
            logger.LogInformation("hello");
			return View(model);
		}
	}
}