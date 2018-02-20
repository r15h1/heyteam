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

		public PlayersApiController(Club club, IMemberQuery memberQuery) {
			this.club = club;
			this.memberQuery = memberQuery;
		}

		[HttpGet]
		public IActionResult Get(GenericSearchModel model) {
			var results = memberQuery.SearchPlayers(model.Query, model.Page, model.Limit) ?? new List<PlayerSearchResult>(); ;
			return new JsonResult(results?.Select(t => new { id = t.PlayerId, text = $"{t.PlayerName} - {t.SquadName} (#{t.SquadNumber})"}));
		}
	}
}