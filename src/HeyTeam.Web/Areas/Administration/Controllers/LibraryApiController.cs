using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HeyTeam.Web.Areas.Administration.Controllers {

	[Authorize(Policy = "Administrator")]
	[Produces("application/json")]
	[Area("Administration")]
	[Route("[area]/api/library")]
	public class LibraryApiController : Controller
    {
		private readonly Club club;
		private readonly ILibraryQuery libraryQuery;

		public LibraryApiController(Club club, ILibraryQuery libraryQuery) {
			this.club = club;
			this.libraryQuery = libraryQuery;
		}

		[HttpGet]
		public IActionResult Get(LibrarySearchModel model) {
			var results = libraryQuery.Search(club.Guid, model.Query, model.Page, model.Limit) ?? new List<string>(); ;
			return new JsonResult(results);
		}
    }
}