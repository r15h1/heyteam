using HeyTeam.Core;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EvaluationViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/evaluations")]
    public class EvaluatonsApiController : Controller
    {
        private readonly Club club;
        private readonly IReportDesigner reportDesigner;

        public EvaluatonsApiController(Club club, IReportDesigner reportDesigner)
        {
            this.club = club;
            this.reportDesigner = reportDesigner;
        }

        [HttpPost("terms/{termId:guid}/designs/new")]
        public IActionResult NewReportCardDesign([FromBody] ReportCardDesignViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();                
                return BadRequest(errors);
            }

            var result = reportDesigner.CreateReportCardDesign(new NewReportCardDesignRequest {
                ClubId = club.Guid,
                Name = model.ReporCardDesignName,
                TermId = model.TermId
            });

            if(!result.Response.RequestIsFulfilled)
                return BadRequest(result.Response.Errors);

            return Ok();
        }
    }
}