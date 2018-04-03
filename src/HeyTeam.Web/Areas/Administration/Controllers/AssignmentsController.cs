using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Web.Models.Assignments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
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
        private readonly IMemberQuery memberQuery;
        private readonly ILibraryQuery libraryQuery;

        public AssignmentsController(Club club, ISquadQuery squadQuery, IMemberQuery memberQuery, ILibraryQuery libraryQuery)
        {
            this.club = club;
            this.squadQuery = squadQuery;
            this.memberQuery = memberQuery;
            this.libraryQuery = libraryQuery;
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

        [HttpPost("new")]
        public IActionResult New(AssignmentDetailsViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
                model.SquadList = GetSquadList();
                model.SelectedPlayerList = GetSelectedPlayerList(model.Players);

                return View("Create", model);
            //}
            //return View("Create", model);
            //return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetSquadList()
        {
            var clubSquads = squadQuery.GetSquads(club.Guid);
            var squadList = clubSquads.Select(s => new SelectListItem { Text = $"{s.Name}", Value = s.Guid.ToString() })
                                    .OrderBy(s => s.Text)
                                    .ToList();
            return squadList;
        }

        private string GetSelectedPlayerList(IEnumerable<Guid> selectedPlayers)
        {
            if (selectedPlayers == null || !selectedPlayers.Any())
                return null;

            var clubSquads = squadQuery.GetSquads(club.Guid);
            var members = memberQuery.GetMembers(clubSquads.Select(s => s.Guid));
            var players = members.SelectMany(m => m.Members)
                                    .Where(m => m.Membership.Equals("Player") && selectedPlayers.Contains(m.Guid))
                                    .Select(m => new { Text = $"{m.Name}", id = m.Guid.ToString() })
                                    .OrderBy(s => s.Text)
                                    .ToList();

            return JsonConvert.SerializeObject(players, 
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
            );
        }

        private string GetSelectedTrainingMaterialList(IEnumerable<Guid> selectedTrainingMaterials)
        {
            if (selectedTrainingMaterials == null || !selectedTrainingMaterials.Any())
                return null;

            var trainingMaterials = libraryQuery.GetTrainingMaterials(club.Guid);
            return JsonConvert.SerializeObject(
                    trainingMaterials.Where(t => selectedTrainingMaterials.Contains(t.Guid))
                                    .Select(t => new { Id = t.Guid, Text = t.Title, Thumbnail = t.ThumbnailUrl, ContentType = t.ShortContentType }).ToList(),
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                );
        }
    }
}