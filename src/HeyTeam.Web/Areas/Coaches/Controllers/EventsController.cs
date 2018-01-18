using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HeyTeam.Web.Areas.Coaches.Controllers {
	[Authorize(Policy = "Coach")]
	[Area("Coaches")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class EventsController : Controller
    {
		private readonly Club club;
		private readonly IEventService eventService;
		private readonly IEventQuery eventsQuery;
		private readonly ISquadQuery squadQuery;

		public EventsController(Club club, IEventService eventService, IEventQuery eventsQuery, ISquadQuery squadQuery) {
			this.club = club;
			this.eventService = eventService;
			this.eventsQuery = eventsQuery;
			this.squadQuery = squadQuery;
		}

		[HttpGet("")]
		public ActionResult Index() {
			var events = eventsQuery.GetEventsSummary(club.Guid);
			return View(events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate));
		}

		[HttpGet("{eventId:guid}")]
		public ActionResult Details(string eventId) {
			var events = eventsQuery.GetEventsSummary(club.Guid);
			return View(events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate));
		}
	}
}