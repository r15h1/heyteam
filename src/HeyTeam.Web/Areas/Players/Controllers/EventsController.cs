using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Areas.Coaches.Models;
using HeyTeam.Web.Models;
using HeyTeam.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Players.Controllers {
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class EventsController : Controller {
		private readonly Club club;
		private readonly IEventService eventService;
		private readonly IEventQuery eventsQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;

		public EventsController(Club club, IEventService eventService, IEventQuery eventsQuery, ISquadQuery squadQuery, IMemberQuery memberQuery) {
			this.club = club;
			this.eventService = eventService;
			this.eventsQuery = eventsQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
		}

		[HttpGet("")]
		public ActionResult Index(Guid memberId) {
			var squads = GetSquadList(memberId);
			var model = new EventsViewModel { Squads = squads, PlayerId = memberId };
			return View(model);
		}

		[HttpGet("{eventId:guid}")]
		public ActionResult Details(Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
			var model = MapEvent(@event);
			return View(model);
		}

		private EventDetailsViewModel MapEvent(Event @event) => new EventDetailsViewModel {
			EndDate = @event.EndDate,
			EventId = @event.Guid,
			Location = @event.Location,
			StartDate = @event.StartDate,
			Title = @event.Title,
			Squads = @event.Squads.Select(s => s.Guid),
			SquadList = GetSquadList(),
			TrainingMaterials = @event.TrainingMaterials,
			EventTypeDescription = @event.EventType.GetDescription()
		};

		private List<SelectListItem> GetSquadList(Guid? memberId = null) {
			var squads = squadQuery.GetSquads(club.Guid);

			if (!memberId.IsEmpty()) {
				var player = memberQuery.GetPlayer(memberId.Value);
				squads = squadQuery.GetSquads(club.Guid).Where(s => player.SquadId == s.Guid);
			}
			
			var squadList = squads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}
	}
}