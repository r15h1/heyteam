using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize(Policy = "Administrator")]
	[Route("[controller]")]
	public class EventsController : Controller {
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

		[HttpGet("new")]
		public ActionResult Create() {
			var model = new EventViewModel { SquadList = GetSquadList() };
			return View(model);
		}

		// POST: Sessions/Create
		[HttpPost("new")]
		[ValidateAntiForgeryToken]
		public ActionResult Create(EventViewModel model) {
			if (!ModelState.IsValid) {
				model.SquadList = GetSquadList();
				return View(model);
			}
			
			try {
				EventSetupRequest request = Map(model);
				var response = eventService.CreateEvent(request);
				if (!response.RequestIsFulfilled) {
					foreach (var error in response.Errors)
						ModelState.AddModelError("", error);

					model.SquadList = GetSquadList();
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			} catch {
				return View();
			}
		}

		private List<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}

		private EventSetupRequest Map(EventViewModel model) => new EventSetupRequest {
			ClubId = club.Guid,
			EndDate = model.EndDate,
			Location = model.Location,
			StartDate = model.StartDate,
			Title = model.Title,
			Squads = model.Squads,
			TrainingMaterials = model.TrainingMaterials,
			EventId = model.EventId
		};

		[HttpGet("{eventId}")]
		public ActionResult Edit([FromRoute]string eventId) {
			var @event = eventsQuery.GetEvent(Guid.Parse(eventId));
			var model = MapEvent(@event);
			return View(model);
		}

		private EventViewModel MapEvent(Event @event) => new EventViewModel {
			EndDate = @event.EndDate,
			EventId = @event.Guid,
			Location = @event.Location,
			StartDate = @event.StartDate,
			Title = @event.Title,
			Squads = @event.Squads.Select(s => s.Guid),
			SquadList = GetSquadList(),
			TrainingMaterials = @event.TrainingMaterials?.Select(t => t.Guid),
			SelectedTrainingMaterialList = JsonConvert.SerializeObject(
					@event.TrainingMaterials?.Select(t => new { Id = t.Guid, Text = t.Title, Thumbnail = t.ThumbnailUrl, ContentType = t.ShortContentType }).ToList(),
					new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver()}
				)
		};

		// POST: Sessions/Edit/5
		[HttpPost("{eventId}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel model) {
			if (!ModelState.IsValid) {
				model.SquadList = GetSquadList();
				return View(model);
			}

			try {
				EventSetupRequest request = Map(model);
				var response = eventService.UpdateEvent(request);
				if (!response.RequestIsFulfilled) {
					foreach (var error in response.Errors)
						ModelState.AddModelError("", error);

					model.SquadList = GetSquadList();
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			} catch {
				return View();
			}
		}


        // POST: Sessions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid eventId) {
            try {
				var response = eventService.DeleteEvent(new EventDeleteRequest { ClubId = club.Guid, EventId = eventId });
				return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }
    }
}