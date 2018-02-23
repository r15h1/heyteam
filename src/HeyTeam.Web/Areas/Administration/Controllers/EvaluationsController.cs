using HeyTeam.Core;
using HeyTeam.Lib.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
	[Area("Administration")]
	[Route("[area]/[controller]")]
	public class EvaluationsController : Controller
    {
        private readonly Club club;
        private readonly IEvaluationQuery evaluationQuery;

        public EvaluationsController(Club club, IEvaluationQuery evaluationQuery)
        {
            this.club = club;
            this.evaluationQuery = evaluationQuery;
        }

        [HttpGet("")]
		[HttpGet("terms")]
		public IActionResult Terms()
        {
            var terms = evaluationQuery.GetTerms(club.Guid);
            return View(terms);
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