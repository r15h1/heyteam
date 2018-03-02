using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EvaluationViewModels;
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
		private readonly IEvaluationService evaluationService;
		private readonly IEvaluationQuery evaluationQuery;

        public EvaluationsController(Club club, IEvaluationService evaluationService, IEvaluationQuery evaluationQuery)
        {
            this.club = club;
			this.evaluationService = evaluationService;
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

		[HttpPost("terms/new")]
		public IActionResult NewTerm(TermModel model) {
			if (!ModelState.IsValid) {
				return View(model);
			} else if (model.EndDate <= model.StartDate) {
				ModelState.AddModelError("", "End Date must be greater than the Start Date");
				return View(model);
			}

			var response = evaluationService.CreateTerm (
				new TermSetupRequest { 
					ClubId = club.Guid, EndDate = model.EndDate, 
					StartDate = model.StartDate, Title = model.Title 
				}
			);
			
			if(!response.RequestIsFulfilled) {
				foreach(var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction(nameof(Terms));
		}

		[HttpGet("terms/{termId:guid}")]
		public IActionResult TermDetails(Guid termId) {
			return View();
		}

		[HttpGet("terms/{termId:guid}/designs")]
		public IActionResult ReportCardDesigns(Guid termId) {
            var term = evaluationQuery.GetTerm(termId);
            var model = new ReportCardDesignListViewModel { TermInfo = new TermInfoModel { TermId = term.Guid, EndDate = term.EndDate, StartDate = term.StartDate, Title = term.Title } };
            return View(model);
		}     
    }
}