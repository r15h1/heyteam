using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.Assignments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers
{
    [Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/[controller]")]
    public class AssignmentsController : Controller
    {
        private readonly Club club;
        private readonly ISquadQuery squadQuery;

        public AssignmentsController(Club club, ISquadQuery squadQuery)
        {
            this.club = club;
            this.squadQuery = squadQuery;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            var model = new AssignmentDetailsViewModel { SquadList = GetSquadList() };
            return View("Create", model);
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