using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/assignments")]
	public class AssignmentsApiController : Controller
    {
		private readonly Club club;
		private readonly IAssignmentQuery assignmentQuery;

		public AssignmentsApiController(Club club, IAssignmentQuery assignmentQuery){
			this.club = club;
			this.assignmentQuery = assignmentQuery;
		}

		[HttpGet("")]
		public IActionResult GetAssignments(AssignmentSearchModel model) {
			var assignments = assignmentQuery.GetAssignments(new AssignmentsRequest { ClubId = club.Guid, Month = model.Month, Year = model.Year, Squads = model.Squads });
			return new JsonResult(assignments);
		}
	}
}