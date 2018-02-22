using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
	[Area("Administration")]
	[Route("[area]/[controller]")]
	public class EvaluationsController : Controller
    {
        [HttpGet("")]
		[HttpGet("terms")]
		public IActionResult Terms()
        {
            return View();
        }

		[HttpGet("terms/new")]
		public IActionResult NewTerm() {
			return View();
		}

		[HttpGet("terms/{termId:guid}")]
		public IActionResult TermDetails(Guid termId) {
			return View();
		}

		[HttpGet("terms/{termId:guid}/designer")]
		public IActionResult Designer() {
			return View();
		}
	}
}