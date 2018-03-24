using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Identity;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize]
    [Produces("application/json")]
    [Route("api/events")]
    public class EventsApiController : Microsoft.AspNetCore.Mvc.Controller
    {
		private readonly Club club;
		private readonly IEventQuery eventsQuery;
		private readonly IEventService eventService;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IMemberQuery memberQuery;

		public EventsApiController(Club club, IEventQuery eventsQuery, IEventService eventService, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) {
			this.club = club;
			this.eventsQuery = eventsQuery;
			this.eventService = eventService;
			this.userManager = userManager;
			this.memberQuery = memberQuery;
		}

		[HttpGet("")]
		public IActionResult GetEvents(Guid? squad, Guid? playerId, int month, int year) {
			var eventRequest = new EventsRequest() { 
				ClubId = club.Guid,
				Month = month,
				Year = year,
				SquadId = squad,
				PlayerId = playerId
			};

			var events = eventsQuery.GetEventsSummary(eventRequest);
			var response = events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate).Select(e => new { 
				EventId = e.Guid, Title = e.Title, TrainingMaterialsCount = e.TrainingMaterialsCount ,
				Squads = e.Squads, Location = e.Location, FormattedStartDate = $"{e.StartDate.ToString("dd MMM yyyy h:mm tt")}",
				FormattedEndDate = $"{e.EndDate.ToString("dd MMM yyyy h:mm tt")}",
				StartDate = e.StartDate, EndDate = e.EndDate,
				Attendance = e.Attendance,
				EventType = (byte) e.EventType,
				EventTypeDescription = e.EventType.GetDescription()
			}).ToList();

			return Json(response);
		}

		[HttpGet("{eventId:guid}/attendance")]
		public IActionResult Attendance(Guid eventId) {
			var eventPlayers = eventsQuery.GetPlayersByEvent(eventId);
			return Json(eventPlayers);
		}

		[HttpPost("attendance")]
		public IActionResult Attendance([FromBody] EventAttendanceViewModel attendance) {
			if(!ModelState.IsValid)
				return BadRequest();
		
			var request = new EventAttendanceRequest {
				ClubId = club.Guid,
				EventId = attendance.EventId,
				PlayerId = attendance.PlayerId,
				SquadId = attendance.SquadId,
				Attendance = attendance.Attendance
			};

			var response = eventService.UpdateEventAttendance(request);

			if (!response.RequestIsFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return BadRequest();
			}

			return Ok();
		}

		[HttpPost("emailReport")]
		public IActionResult EmailReport([FromBody] EmailReportViewModel model) {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var request = new EmailReportRequest {
				ClubId = club.Guid,
				EventId = model.EventId,
				EmailAddresses = model.EmailAddress
			};

			if (model.SendMeACopy.HasValue && model.SendMeACopy.Value) {
				var user = userManager.GetUserAsync(User).Result;
				request.EmailAddresses.Add(user.Email);
			}

			var response = eventService.EmailEventReport(request);
			return response.RequestIsFulfilled ? Ok() : BadRequest(response.Errors) as IActionResult;
		}

		[HttpGet("{eventId:guid}/library/views")]
		public IActionResult GetTrainingMaterialViews(Guid eventId)
		{
			var evnt = eventsQuery.GetEvent(eventId);
			var views = eventsQuery.GetTrainingMaterialViews(eventId);
			var squads = memberQuery.GetMembers(evnt.Squads.Select(s => s.Guid));
			return new JsonResult(new { results = new { squads, trainingMaterialViews = views } });
		}
	}
}