using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.FeedbackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly Club club;
        private readonly ISquadQuery squadQuery;

        public FeedbackController(Club club, ISquadQuery squadQuery)
        {
            this.club = club;
            this.squadQuery = squadQuery;
        }

        [Authorize(Policy = "Administrator")]
        [Area("Administration")]
        [Route("[area]/[controller]")]
        public IActionResult Index()
        {
            var squads = GetSquadList().OrderBy(s => s.Text).Prepend(new SelectListItem { Text = "Select", Value = "", Disabled = true, Selected=true}).ToList();
            var model = new FeedbackViewModel { Squads = squads };
            return View(model);
        }

        private List<SelectListItem> GetSquadList()
        {
            var clubSquads = squadQuery.GetSquads(club.Guid);
            var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
                                    .OrderBy(s => s.Text)
                                    .ToList();
            return squadList;
        }
    }
}