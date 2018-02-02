﻿using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Areas.Coaches.Models;
using HeyTeam.Web.Models.EventsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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
			var squads = GetSquadList();
			var model = new EventsViewModel { Squads = squads };
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
			TrainingMaterials = @event.TrainingMaterials
		};

		private List<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
                                    .Prepend(new SelectListItem { Text = "All", Value = "" })
									.ToList();
			return squadList;
		}

		[HttpGet("{eventId:guid}/attendance")]
		public ActionResult Attendance(Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
			var eventPlayers = eventsQuery.GetPlayersByEvent(eventId);
			var model = new EventAttendanceModel {
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
		public ActionResult Reviews(Guid eventId) {			
			var @event = eventsQuery.GetEvent(eventId);
			var reviews = eventsQuery.GetEventReviews(eventId);
			//var notYetReviewed = @event.Squads.Select(s => s.Guid).Except(reviews.SelectMany(r => r.Squads));
			//var squadsNotYetReviewed = @event.Squads.Where(s => notYetReviewed.Contains(s.Guid));

			var model = new EventReviewViewModel {
				EventTitle = @event.Title,
				EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
				Reviews = reviews,
				//SquadsNotYetReviewed = squadsNotYetReviewed
			};

			return View(model);
		}

		[HttpGet("{eventId:guid}/reviews/new")]
		public ActionResult NewReview(Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
			var squadsNotYetReviewed = GetNotYetReviewedSquads(@event);

			var model = new NewEventReviewViewModel {
				EventTitle = @event.Title,
				EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
				SquadsNotYetReviewed = squadsNotYetReviewed
			};

			return View(model);
		}

		private List<SelectListItem> GetNotYetReviewedSquads(Event @event) {
			var reviews = eventsQuery.GetEventReviews(@event.Guid);
			var notYetReviewed = @event.Squads.Select(s => s.Guid).Except(reviews.SelectMany(r => r.Squads).Select(r => r.Guid));
			var squadsNotYetReviewed = @event.Squads
											.Where(s => notYetReviewed.Contains(s.Guid))
											.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
											.OrderBy(s => s.Text).ToList();
			return squadsNotYetReviewed;
		}

		[HttpPost("{eventId:guid}/reviews/new")]
		public ActionResult NewReview(NewEventReviewViewModel model) {
			if(!ModelState.IsValid) {
				UpdateReviewModel(model);
				return View(model);
			}
			var eventReviewRequest = new NewEventReviewRequest {
				ClubId = club.Guid,
				CoachId = model.MemberId,
				DifferentNextTime = model.DifferentNextTime,
				EventId = model.EventId,
				Opportunities = model.Opportunities,
				Squads = model.Squads,
				Successes= model.Successes
			};

			var response = eventService.AddEventReview(eventReviewRequest);
			if(!response.RequestIsFulfilled) {
				UpdateReviewModel(model);
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

				return View(model);
			}

			return RedirectToAction(nameof(Reviews));
		}

		private void UpdateReviewModel(NewEventReviewViewModel model) {
			var @event = eventsQuery.GetEvent(model.EventId);
			var squadsNotYetReviewed = GetNotYetReviewedSquads(@event);
			model.EventTitle = @event.Title;
			model.EventDetails = $"{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}";
			model.SquadsNotYetReviewed = squadsNotYetReviewed;			
		}
	}
}