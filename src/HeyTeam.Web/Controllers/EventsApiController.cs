using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
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

		public EventsApiController(Club club, IEventQuery eventsQuery) {
			this.club = club;
			this.eventsQuery = eventsQuery;
		}

		[HttpGet("")]
		public IEnumerable<EventSummary> GetEvents(IEnumerable<Guid> squads, int month, int year) {
			var eventRequest = new EventsRequest() { 
				ClubId = club.Guid,
				Month = month,
				Year = year,
				Squads = squads
			};

			var events = eventsQuery.GetEventsSummary(eventRequest);
			return events;
		}
    }
}