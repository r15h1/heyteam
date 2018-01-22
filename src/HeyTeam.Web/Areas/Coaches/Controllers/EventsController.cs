using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Areas.Coaches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
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
			var squads = GetSquadList();
			var model = new EventsViewModel { EventSummary = events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate), Squads = squads };
			return View(model);
		}

		[HttpGet("{eventId:guid}")]
		public ActionResult Details(string eventId) {
			var events = eventsQuery.GetEventsSummary(club.Guid);
			return View(events.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate));
		}

		private List<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}
	}
}