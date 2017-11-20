using HeyTeam.Core.Entities;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.UseCases.Events;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Controllers {
	[Authorize]
	[Route("[controller]")]
	public class EventsController : Controller {
		private readonly Club club;
		private readonly ISquadQuery squadQuery;

		public EventsController(Club club, ISquadQuery squadQuery) {
			this.club = club;
			this.squadQuery = squadQuery;
		}

        [HttpGet("")]
        public ActionResult Index() {
            return View(new List<EventListViewModel>());
        }

        // GET: Sessions/Details/5
        public ActionResult Details(int id) {
            return View();
        }

		[HttpGet("new")]
		public ActionResult New() {
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
        public ActionResult New(NewEventViewModel model) {
			if (!ModelState.IsValid) {
				var clubSquads = squadQuery.GetSquads(club.Guid);
				var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
										.OrderBy(s => s.Text)
										.ToList();
				model.SquadList = squadList;
				return View(model);
			}
				

            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

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