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

namespace HeyTeam.Web.Areas.Players.Controllers {
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}/[controller]")]
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
		

        [HttpGet("player-report-cards")]
        public IActionResult PlayerReportCards(Guid memberId)
        {
			var reportcards = evaluationQuery.GetPlayerReportCards(club.Guid, memberId);			
			return View(reportcards);
        }

		private IEnumerable<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}

		[HttpGet("player-report-cards/{playerReportCardId:guid}")]
		public IActionResult PlayerReportCardDetails(Guid playerReportCardId) {
			var reportCard = evaluationQuery.GetPlayerReportCardDetails(club.Guid, playerReportCardId);
			return View(reportCard);
		}
	}
}