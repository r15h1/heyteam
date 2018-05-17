using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Core;
using HeyTeam.Core.Models;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers
{
	[Authorize]
	[Produces("application/json")]
    [Route("api/players")]
    public class PlayersApiController : Microsoft.AspNetCore.Mvc.Controller
    {
		private readonly Club club;
		private readonly IMemberQuery memberQuery;
		private readonly IFeedbackQuery feedbackQuery;

		public PlayersApiController(Club club, IMemberQuery memberQuery, IFeedbackQuery feedbackQuery) {
			this.club = club;
			this.memberQuery = memberQuery;
			this.feedbackQuery = feedbackQuery;
		}

		[HttpGet]
		public IActionResult Get(GenericSearchModel model) {
			var results = memberQuery.SearchPlayers(model.Query, model.Page, model.Limit) ?? new List<PlayerSearchResult>(); ;
			return new JsonResult(results?.Select(t => new { id = t.PlayerId, text = $"{t.PlayerName} {t.SquadName}"}));
		}

		[HttpGet("{playerId:guid}/feedback")]
		public IActionResult GetFeedback(Guid playerId, int month, int year) {
			var feedbackList = feedbackQuery.GetFeedbackList(new PlayerFeedbackListRequest { 
				ClubId = club.Guid, PlayerId = playerId, Year = year, Month = month
			});
			return new JsonResult(feedbackList);
		}
	}
}