using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
using HeyTeam.Web.Models;
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
		public ActionResult Index(Guid memberId) {
			var squads = GetSquadList(memberId);
			var model = new EventsViewModel { Squads = squads };
			return View(model);
		}

		[HttpGet("{eventId:guid}")]
		public ActionResult Details(Guid memberId, Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
			var model = MapEvent(@event, memberId);
			return View(model);
		}

		private EventDetailsViewModel MapEvent(Event @event, Guid memberId) => new EventDetailsViewModel {
			EndDate = @event.EndDate,
			EventId = @event.Guid,
			Location = @event.Location,
			StartDate = @event.StartDate,
			Title = @event.Title,
			Squads = @event.Squads.Select(s => s.Guid),
			SquadList = GetSquadList(memberId),
			TrainingMaterials = @event.TrainingMaterials,
			EventType = @event.EventType,
			EventTypeDescription = @event.EventType.GetDescription()
		};

		private List<SelectListItem> GetSquadList(Guid memberId) {
			var memberSquads = squadQuery.GetMemberSquads(memberId, Membership.Coach);
            var squadList = memberSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() });									
            if (squadList.Count() > 1)
                squadList = squadList.Prepend(new SelectListItem { Text = "All", Value = "" });

            return squadList.OrderBy(s => s.Text).ToList();
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
				EventPlayers = eventPlayers,
				EventType = @event.EventType.GetDescription()
			};
			return View(model);
		}

		[HttpGet("{eventId:guid}/reviews")]
		public ActionResult Reviews(Guid eventId) {			
			var @event = eventsQuery.GetEvent(eventId);
			var reviews = eventsQuery.GetEventReviews(eventId);
            var squadsNotYetReviewed = eventsQuery.GetUnReviewedSquads(eventId);

			var model = new EventReviewViewModel {
				EventTitle = @event.Title,
				EventDetails = $"{@event.EventType.GetDescription()} {@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
				Reviews = reviews,
				SquadsNotYetReviewed = squadsNotYetReviewed
			};

			return View(model);
		}

		[HttpGet("{eventId:guid}/reviews/new")]
		public ActionResult NewReview(Guid eventId) {
			var @event = eventsQuery.GetEvent(eventId);
			var squadsNotYetReviewed = GetNotYetReviewedSquads(@event);

            if(squadsNotYetReviewed?.Count() == 0)
                return RedirectToAction(nameof(Reviews));

            var model = new NewEventReviewViewModel {
				EventTitle = @event.Title,
				EventDetails = $"{@event.EventType.GetDescription()}<br/>{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}",
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
			model.EventDetails = $"{@event.EventType.GetDescription()}<br/>{@event.StartDate.ToString("ddd dd-MMM-yyyy h:mm tt")}<br/>{@event.Location}<br/>{string.Join(", ", @event.Squads.Select(s => s.Name))}";
			model.SquadsNotYetReviewed = squadsNotYetReviewed;			
		}

		[HttpGet("{eventId:guid}/tracker")]
		public ActionResult Tracker(Guid memberId, Guid eventId) {			
			return View();
		}
	}
}