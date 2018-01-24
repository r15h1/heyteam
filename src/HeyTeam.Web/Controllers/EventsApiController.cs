using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers
{
	[Authorize]
    [Produces("application/json")]
    [Route("api/events")]
    public class EventsApiController : Controller
    {
		private readonly Club club;
		private readonly IEventQuery eventsQuery;
		private readonly IEventService eventService;

		public EventsApiController(Club club, IEventQuery eventsQuery, IEventService eventService) {
			this.club = club;
			this.eventsQuery = eventsQuery;
			this.eventService = eventService;
		}

		[HttpGet("")]
		public IEnumerable<EventSummary> GetEvents(Guid? squad, int month, int year) {
			var eventRequest = new EventsRequest() { 
				ClubId = club.Guid,
				Month = month,
				Year = year,
				Squad = squad
			};

			var events = eventsQuery.GetEventsSummary(eventRequest);
			return events;
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

			var response = eventService.UpdateAttendance(request);

			if (!response.RequestIsFulfilled) {
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return BadRequest();
			}

			return Ok();
		}
	}
}