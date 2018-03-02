using HeyTeam.Core;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EvaluationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/report-card-designer")]
    public class ReportDesignerApiController : Controller
    {
        private readonly Club club;
        private readonly IReportDesigner reportDesigner;

        public ReportDesignerApiController(Club club, IReportDesigner reportDesigner)
        {
            this.club = club;
            this.reportDesigner = reportDesigner;
        }

        [HttpPost("new")]
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
    }
}