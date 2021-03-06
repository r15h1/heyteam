﻿using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HeyTeam.Web.Areas.Players.Controllers {
	[Produces("application/json")]    
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("api/[area]/{memberid:guid}")]
	public class DashboardApiController : Controller
    {
		private readonly Club club;
		private readonly IEventQuery eventQuery;
		private readonly IFeedbackQuery feedbackQuery;
        private readonly ISquadQuery squadQuery;
		private readonly IAssignmentQuery assignmentQuery;

		public DashboardApiController(Club club, IEventQuery eventQuery, IFeedbackQuery feedbackQuery, ISquadQuery squadQuery, IAssignmentQuery assignmentQuery) {
			this.club = club;
			this.eventQuery = eventQuery;
			this.feedbackQuery = feedbackQuery;
            this.squadQuery = squadQuery;
			this.assignmentQuery = assignmentQuery;
		}
		
		[HttpGet("upcoming-events")]
		public IActionResult GetUpcomingEvents(DashboardModel model)
		{
			var events = eventQuery.GetUpcomingEvents(new UpcomingEventsRequest { Membership = Membership.Player, MemberId = model.MemberId, ClubId = club.Guid });
			var response = events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate).Select(e => new {
				EventId = e.Guid, Title = e.Title, TrainingMaterialsCount = e.TrainingMaterialsCount,
				Squads = e.Squads, Location = e.Location, FormattedStartDate = $"{e.StartDate.ToString("dd MMM yyyy h:mm tt")}",
				FormattedEndDate = $"{e.EndDate.ToString("dd MMM yyyy h:mm tt")}",
				StartDate = e.StartDate, EndDate = e.EndDate,
				Attendance = e.Attendance,
				EventType = (byte)e.EventType,
				EventTypeDescription = e.EventType.GetDescription()
			}).ToList();
			return new JsonResult(response);
		}

		[HttpGet("latest-feedback")]
		public IActionResult GetLatestFeedback(DashboardModel model) {
			var response = feedbackQuery.GetLatestFeedback(new LatestFeedbackRequest { ClubId = club.Guid, MemberId = model.MemberId, Membership = Membership.Player });
			return new JsonResult(response);
		}

        [HttpGet("my-squads")]
        public IActionResult GetMySquads(DashboardModel model)
        {
            var squads = squadQuery.GetMemberSquads(model.MemberId, Membership.Player);            
            return new JsonResult(squads);
        }

		[HttpGet("upcoming-assignments")]
		public IActionResult GetUpcomingAssignments(DashboardModel model) {
			var assignments = assignmentQuery.GetAssignments(new AssignmentsRequest { ClubId = club.Guid, PlayerId = model.MemberId, Date = DateTime.Today });
			return new JsonResult(assignments);
		}
	}
}