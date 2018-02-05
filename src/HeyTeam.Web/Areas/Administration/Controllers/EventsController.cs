﻿using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models;
using HeyTeam.Web.Models.EventsViewModels;
using HeyTeam.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers {
	[Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/[controller]")]
    public class EventsController : Controller {
		private readonly Club club;
		private readonly IEventService eventService;
		private readonly IEventQuery eventsQuery;
		private readonly ISquadQuery squadQuery;
		private readonly IMemberQuery memberQuery;
		private const string Temp_Data_Error = "Temp_Data_Error";

		public EventsController(Club club, IEventService eventService, IEventQuery eventsQuery, ISquadQuery squadQuery, IMemberQuery memberQuery) {
			this.club = club;
			this.eventService = eventService;
			this.eventsQuery = eventsQuery;
			this.squadQuery = squadQuery;
			this.memberQuery = memberQuery;
		}

		[HttpGet("")]
		public ActionResult Index() {
            var squads = GetSquadList().OrderBy(s => s.Text).Prepend(new SelectListItem { Text = "All", Value = "" }).ToList();
            var model = new EventsViewModel { Squads = squads };
            return View(model);
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

		[HttpGet("{eventId:guid}")]
		public ActionResult Edit(Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
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
        [HttpPost("delete")]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(Guid eventId) {
            try {
				var response = eventService.DeleteEvent(new EventDeleteRequest { ClubId = club.Guid, EventId = eventId});
				return Ok();
            }
            catch {
                return BadRequest();
            }
        }

        [HttpGet("{eventId:guid}/attendance")]
        public ActionResult Attendance(Guid eventId)
        {
            var @event = eventsQuery.GetEvent(eventId);
            var eventPlayers = eventsQuery.GetPlayersByEvent(eventId);
            var model = new EventAttendanceModel
            {
                EndDate = @event.EndDate,
                EventId = @event.Guid,
                Location = @event.Location,
                StartDate = @event.StartDate,
                Title = @event.Title,
                EventPlayers = eventPlayers
            };
            return View(model);
        }

        [HttpGet("{eventId:guid}/reviews")]
        public ActionResult Reviews(Guid eventId)
        {
            var @event = eventsQuery.GetEvent(eventId);
            var reviews = eventsQuery.GetEventReviews(eventId);
            var squadsNotYetReviewed = eventsQuery.GetUnReviewedSquads(eventId);

            var model = new EventReviewViewModel
            {
                EventTitle = @event.Title,
                EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
                Reviews = reviews,
                SquadsNotYetReviewed = squadsNotYetReviewed
            };

            return View(model);
        }

        [HttpGet("{eventId:guid}/reviews/new")]
        public ActionResult NewReview(Guid eventId)
        {
            var @event = eventsQuery.GetEvent(eventId);
            var squadsNotYetReviewed = GetNotYetReviewedSquads(@event);			

			if (squadsNotYetReviewed?.Count() == 0)
                return RedirectToAction(nameof(Reviews));

			var email = User.Identity.Name;
			var members = memberQuery.GetMembersByEmail(club.Guid, email);
			var coach = members?.FirstOrDefault(m => m.Membership == Membership.Coach);			

			if(coach == null || coach.Guid.IsEmpty()) {
				TempData[Temp_Data_Error] = "This user has not been setup as a coach. To submit reviews, add this user as a coach";
				return RedirectToAction(nameof(Reviews));
			}


			var model = new NewEventReviewViewModel
            {
                EventTitle = @event.Title,
                EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
                SquadsNotYetReviewed = squadsNotYetReviewed,
				MemberId = coach.Guid 
            };

            return View(model);
        }

        private List<SelectListItem> GetNotYetReviewedSquads(Event @event)
        {
            var reviews = eventsQuery.GetEventReviews(@event.Guid);
            var notYetReviewed = @event.Squads.Select(s => s.Guid).Except(reviews.SelectMany(r => r.Squads).Select(r => r.Guid));
            var squadsNotYetReviewed = @event.Squads
                                            .Where(s => notYetReviewed.Contains(s.Guid))
                                            .Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
                                            .OrderBy(s => s.Text).ToList();
            return squadsNotYetReviewed;
        }

        [HttpPost("{eventId:guid}/reviews/new")]
        public ActionResult NewReview(NewEventReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                UpdateReviewModel(model);
                return View(model);
            }

            var eventReviewRequest = new NewEventReviewRequest
            {
                ClubId = club.Guid,
                CoachId = model.MemberId,
                DifferentNextTime = model.DifferentNextTime,
                EventId = model.EventId,
                Opportunities = model.Opportunities,
                Squads = model.Squads,
                Successes = model.Successes
            };

            var response = eventService.AddEventReview(eventReviewRequest);
            if (!response.RequestIsFulfilled)
            {
                UpdateReviewModel(model);
                foreach (var error in response.Errors)
                    ModelState.AddModelError("", error);

                return View(model);
            }

            return RedirectToAction(nameof(Reviews));
        }

        private void UpdateReviewModel(NewEventReviewViewModel model)
        {
            var @event = eventsQuery.GetEvent(model.EventId);
            var squadsNotYetReviewed = GetNotYetReviewedSquads(@event);
            model.EventTitle = @event.Title;
            model.EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}";
            model.SquadsNotYetReviewed = squadsNotYetReviewed;
        }
    }
}