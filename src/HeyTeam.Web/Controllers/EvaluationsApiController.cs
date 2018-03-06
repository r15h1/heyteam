using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.ApiModels;
using HeyTeam.Web.Models.EvaluationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		private readonly IEvaluationQuery evaluationQuery;

		public EvaluationsApiController(Club club, IReportDesigner reportDesigner, IEvaluationQuery evaluationQuery)
        {
            this.club = club;
            this.reportDesigner = reportDesigner;
			this.evaluationQuery = evaluationQuery;
		}

        [HttpPost("report-designer/new")]
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
			
			return Ok();
		}
    }
}