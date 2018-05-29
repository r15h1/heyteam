using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using HeyTeam.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Areas.Players.Controllers {
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class HomeController : Controller {
		private readonly Club club;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IMemberQuery memberQuery;

		public HomeController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) {
			this.club = club;
			this.userManager = userManager;
			this.memberQuery = memberQuery;
		}

		[HttpGet]
		public IActionResult Index(Guid memberid) {
			var model = new DashboardModel {
				MemberId = memberid,
				Membership = Membership.Player
			};

			return View(model);
		}
	}
}