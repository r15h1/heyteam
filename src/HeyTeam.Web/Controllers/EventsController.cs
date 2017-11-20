using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize]
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
			var events = eventsQuery.GetEvents(club.Guid);
			var list = new List<EventListViewModel>();
			foreach (var @event in events)
				list.Add(Map(@event));

			return View(list);
        }

		private EventListViewModel Map(Event @event) => new EventListViewModel { 
			EndDate = @event.EndDate,
			EventId = @event.Guid,
			Location = @event.Location,
			StartDate = @event.StartDate,
			Title = @event.Title
		};

		// GET: Sessions/Details/5
		public ActionResult Details(int id) {
            return View();
        }

		[HttpGet("new")]
		public ActionResult Create() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			var model = new NewEventViewModel { SquadList = squadList };
            return View(model);
        }

        // POST: Sessions/Create
        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewEventViewModel model) {
			if (!ModelState.IsValid) {
				var clubSquads = squadQuery.GetSquads(club.Guid);
				var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
										.OrderBy(s => s.Text)
										.ToList();
				model.SquadList = squadList;
				return View(model);
			}
				

            try {
				EventSetupRequest request = Map(model);
				var response = eventService.CreateEvent(request);
				if(!response.RequestIsFulfilled) {
					foreach (var error in response.Errors)
						ModelState.AddModelError("", error);

					return View(model);
				}

                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

		private EventSetupRequest Map(NewEventViewModel model) => new EventSetupRequest {
			ClubId = club.Guid,
			EndDate = model.EndDate,
			Location = model.Location,
			StartDate = model.StartDate,
			Title = model.Title
		};

		// GET: Sessions/Edit/5
		public ActionResult Edit(int id) {
            return View();
        }

        // POST: Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Sessions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }
    }
}