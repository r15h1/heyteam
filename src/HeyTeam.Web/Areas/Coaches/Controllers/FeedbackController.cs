using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Repositories;
using HeyTeam.Web.Models.FeedbackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Coaches.Controllers
{
	[Authorize(Policy = "Coach")]
	[Area("Coaches")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class FeedbackController : Controller {
		private readonly Club club;
		private readonly ISquadQuery squadQuery;
		private readonly IFeedbackQuery feedbackQuery;
		private readonly IFeedbackRepository feedbackRepository;
		private readonly IMemberQuery memberQuery;

		public FeedbackController(Club club, ISquadQuery squadQuery, IFeedbackQuery feedbackQuery, IFeedbackRepository feedbackRepository, IMemberQuery memberQuery) {
			this.club = club;
			this.squadQuery = squadQuery;
			this.feedbackQuery = feedbackQuery;
			this.feedbackRepository = feedbackRepository;
			this.memberQuery = memberQuery;
		}

		[HttpGet("")]
		public IActionResult Index() {
			var squads = GetSquadList().OrderBy(s => s.Text).Prepend(new SelectListItem { Text = "Select", Value = "", Disabled = true, Selected = true }).ToList();
			var model = new FeedbackViewModel { Squads = squads };
			return View(model);
		}

		private List<SelectListItem> GetSquadList() {
			var clubSquads = squadQuery.GetSquads(club.Guid);
			var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
									.OrderBy(s => s.Text)
									.ToList();
			return squadList;
		}

		[HttpGet("{feedbackId:guid}")]
		public IActionResult FeedbackChain(Guid feedbackId) {
			var feedbackChain = feedbackQuery.GetFeedbackChain(
				new FeedbackChainRequest { ClubId = club.Guid, FeedbackId = feedbackId }
			);
			var viewModel = new FeedbackChainModel { FeedbackChain = feedbackChain, IsMember = GetCoach() != null };
			return View(viewModel);
		}

		[HttpPost("{feedbackId:guid}")]
		public IActionResult AddComment(NewCommentModel model) {
			if (ModelState.IsValid) {
				var coach = GetCoach();

				var request = new AddCommentRequest {
					ClubId = club.Guid,
					Comment = model.Comment,
					FeedbackId = model.FeedbackId,
					PostedBy = $"{coach.FirstName} {coach.LastName}",
					PosterId = coach.Guid,
					Membership = Membership.Coach
				};

				feedbackRepository.AddComment(request);
			}
			var feedbackChain = feedbackQuery.GetFeedbackChain(
				new FeedbackChainRequest { ClubId = club.Guid, FeedbackId = model.FeedbackId }
			);
			var viewModel = new FeedbackChainModel { FeedbackChain = feedbackChain, IsMember = GetCoach() != null };
			return View("FeedbackChain", viewModel);
		}

		private Member GetCoach() {
			var email = User.Identity.Name;
			var members = memberQuery.GetMembersByEmail(club.Guid, email);
			return members?.FirstOrDefault(m => m.Membership == Membership.Coach);
		}
	}
}