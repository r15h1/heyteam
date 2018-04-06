using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Util;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/assignments")]
	public class AssignmentsApiController : Controller
    {
		private readonly Club club;
		private readonly IAssignmentQuery assignmentQuery;
        private readonly IAssignmentService assignmentService;

        public AssignmentsApiController(Club club, IAssignmentQuery assignmentQuery, IAssignmentService assignmentService){
			this.club = club;
			this.assignmentQuery = assignmentQuery;
            this.assignmentService = assignmentService;
        }

		[HttpGet("")]
		public IActionResult GetAssignments(AssignmentSearchModel model) {
			var assignments = assignmentQuery.GetAssignments(new AssignmentsRequest { ClubId = club.Guid, Month = model.Month, Year = model.Year, Squads = model.Squads });
			return new JsonResult(assignments);
		}

        [HttpPost("remove-player")]
        public IActionResult RemovePlayerFromAssignment(Guid playerAssignmentId)
        {
            if (playerAssignmentId.IsEmpty())
            {
                ModelState.AddModelError("", "Player Assignment Id is required");
                return BadRequest(ModelState);
            }

            var response = assignmentService.RemovePlayerFromAssignment(new UnAssignPlayerRequest { ClubId = club.Guid, PlayerAssignmentId = playerAssignmentId });
            if (!response.RequestIsFulfilled)
            {
                foreach(var error in response.Errors)
                    ModelState.AddModelError("", error);

                return BadRequest(ModelState);
            }
            return Ok();
        }
	}
}