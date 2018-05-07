using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Produces("application/json")]
	[Route("api/assignments")]
	public class AssignmentsApiController : Controller
    {
		private readonly Club club;
		private readonly IAssignmentQuery assignmentQuery;
        private readonly IAssignmentService assignmentService;
		private readonly IMemberQuery memberQuery;

		public AssignmentsApiController(Club club, IAssignmentQuery assignmentQuery, IAssignmentService assignmentService, IMemberQuery memberQuery){
			this.club = club;
			this.assignmentQuery = assignmentQuery;
            this.assignmentService = assignmentService;
			this.memberQuery = memberQuery;
		}

		[HttpGet("")]
		public IActionResult GetAssignments(AssignmentSearchModel model) {
			var assignments = assignmentQuery.GetAssignments(
				new AssignmentsRequest { 
					ClubId = club.Guid, Date = model.Date , SquadId = model.SquadId, 
					PlayerId = model.PlayerId });
			return new JsonResult(assignments);
		}

		[HttpGet("{assignmentId:guid}")]
		public IActionResult GetAssignment(Guid assignmentId, Guid? playerId) {
			var assignment = assignmentQuery.GetAssignment(club.Guid, assignmentId );

			if (!playerId.IsEmpty() && assignment != null) {
				Task.Run(() => assignmentService.TrackAssignmentView(
					new AssignmentViewTrackingRequest { ClubId = club.Guid, AssignmentId = assignmentId, PlayerId = playerId.Value }
				));
			}

			return new JsonResult(assignment);
		}

		[HttpPut("{assignmentId:guid}")]
		public IActionResult UpdateAssignment(UpdateDueDateModel model) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			} else if (model.AssignmentId.IsEmpty()) {
				ModelState.AddModelError("", "AssignmentId cannot be empty guid");
				return BadRequest(ModelState);
			}

			var response = assignmentService.UpdateAssignment(
                new AssignmentUpdateRequest {
                    ClubId = club.Guid,
                    AssignmentId = model.AssignmentId.Value,
                    DueDate = model.DueDate.Value,
                    Instructions=model.Instructions,
                    Title = model.Title,
                    TrainingMaterials = model.TrainingMaterials
                });

            if (!response.RequestIsFulfilled)
            {
                foreach(var error in response.Errors)
                    ModelState.AddModelError("", error);

                return BadRequest(ModelState);
            }

			return Ok();
		}

        [HttpPost("{assignmentId:guid}/players")]
        public IActionResult AddPlayer(Guid assignmentId, Guid playerId)
        {
			var email = User.Identity.Name;
			var members = memberQuery.GetMembersByEmail(club.Guid, email);
			var coach = members?.FirstOrDefault(m => m.Membership == Membership.Coach);

			if (coach == null) {
				ModelState.AddModelError("", "Coach could not be resolved");
				return BadRequest(ModelState);
			}

			if (playerId.IsEmpty() || assignmentId.IsEmpty())
            {
                ModelState.AddModelError("", "AssignmentId and PlayerId are required");
                return BadRequest(ModelState);
            }

			var response = assignmentService.AddPlayerToAssignment(new PlayerAssignmentRequest{
				AssignmentId = assignmentId,
				ClubId = club.Guid,
				CoachId = coach.Guid,
				PlayerId = playerId
			});

			if (response.Errors.Any()) {
				foreach (var error in response.Errors){
					ModelState.AddModelError("", error);
				}
				return BadRequest(ModelState);
			}

            return Ok();
        }

		[HttpPost("remove-player")]
        public IActionResult RemovePlayerFromAssignment(Guid assignmentId, Guid playerId)
        {
            if (playerId.IsEmpty() || assignmentId.IsEmpty())
            {
                ModelState.AddModelError("", "Assignment Id is required");
                return BadRequest(ModelState);
            }

            var response = assignmentService.RemovePlayerFromAssignment(new PlayerAssignmentRequest { ClubId = club.Guid, AssignmentId = assignmentId, PlayerId = playerId });
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