﻿using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Search;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.ApiModels;
using HeyTeam.Web.Models.EvaluationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HeyTeam.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/evaluations")]
    public class EvaluationsApiController : Controller
    {
        private readonly Club club;
        private readonly IReportDesigner reportDesigner;
		private readonly IReportDesignerQuery reportDesignerQuery;
		private readonly ITermSearchEngine termSearchEngine;
		private readonly IEvaluationQuery evaluationQuery;

		public EvaluationsApiController(Club club, IReportDesigner reportDesigner, IReportDesignerQuery reportDesignerQuery,
			ITermSearchEngine termSearchEngine, IEvaluationQuery evaluationQuery)
        {
            this.club = club;
            this.reportDesigner = reportDesigner;
			this.reportDesignerQuery = reportDesignerQuery;
			this.termSearchEngine = termSearchEngine;
			this.evaluationQuery = evaluationQuery;
		}

		[HttpGet("report-designs")]
		public IActionResult GetReportDesigns() {
			var reportDesigns = reportDesignerQuery.GetReportCardDesigns(club.Guid);			
			return Ok(new { results = reportDesigns.Select(r => new { ReportDesignId = r.Guid, Name = r.Name }) });
		}

		[HttpPost("report-designs/new")]
        public IActionResult NewReportCardDesign([FromBody] ReportCardDesignViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();                
                return BadRequest(errors);
            }

            var result = reportDesigner.CreateReportCardDesign(new NewReportDesignRequest {
                ClubId = club.Guid,
                Name = model.ReporCardDesignName
            });

            if(!result.Response.RequestIsFulfilled)
                return BadRequest(result.Response.Errors);

            return Ok(result.Guid);
        }        

		[HttpGet("terms")]
		public IActionResult GetTerms(GenericSearchModel model)
		{
			var results = termSearchEngine.Search(new SearchCriteria(club.Guid) {
				Limit = model.Limit, Page = model.Page, 
				SearchEntity = "term", SearchTerm = model.Query
			});
			return Ok(new { results });
		}

		[HttpGet("report-cards")]
		public IActionResult GetReportCards(Guid termId, Guid squadId) {			
			var term = evaluationQuery.GetTerm(termId);
			var reportCards = evaluationQuery.GetPlayerReportCards(club.Guid, termId, squadId);
			return Ok(new { term = new { startDate = term.StartDate, endDate = term.EndDate, 
										title = term.Title,  status = term.TermStatus.ToString()
									},
						results = reportCards }
					);
		}

		[HttpPost("report-cards/new")]
		public IActionResult GenerateReportCards(Guid termId, Guid squadId, Guid playerId, Guid reportDesignId) {
			return Ok();
		}
	}
}