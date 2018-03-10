using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EvaluationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
	[Area("Administration")]
	[Route("[area]/[controller]")]
	public class EvaluationsController : Controller
    {
        private readonly Club club;
		private readonly IEvaluationService evaluationService;
		private readonly IEvaluationQuery evaluationQuery;
        private readonly IReportDesignerQuery reportDesignerQuery;
		private readonly ISquadQuery squadQuery;

		public EvaluationsController(Club club, IEvaluationService evaluationService, 
            IEvaluationQuery evaluationQuery, IReportDesignerQuery reportDesignerQuery, ISquadQuery squadQuery)
        {
            this.club = club;
			this.evaluationService = evaluationService;
			this.evaluationQuery = evaluationQuery;
            this.reportDesignerQuery = reportDesignerQuery;
			this.squadQuery = squadQuery;
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

		[HttpGet("report-card-designer")]
		public IActionResult ReportCardDesigns() {
            var reportDesigns = reportDesignerQuery.GetReportCardDesigns(club.Guid);
            return View(reportDesigns);
		}

        [HttpGet("report-card-designer/{reportDesignId:guid}")]
        public IActionResult ReportCardDesigner(Guid reportDesignId)
        {
            var reportDesign = reportDesignerQuery.GetReportCardDesign(club.Guid, reportDesignId);
            return View(reportDesign);
        }

        [HttpGet("player-report-cards")]
        public IActionResult PlayerReportCards()
        {
			var squads = GetSquadList().OrderBy(s => s.Text).Prepend(new SelectListItem { Text = "select squad...", Value = "", Selected = true, Disabled = true}).ToList();
			return View(squads);
        }

		private IEnumerable<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}
	}
}