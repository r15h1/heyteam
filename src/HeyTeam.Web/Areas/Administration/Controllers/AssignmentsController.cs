using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Core.Services;
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
		private readonly IAssignmentService assignmentService;
        private readonly IAssignmentQuery assignmentQuery;

        public AssignmentsController(Club club, ISquadQuery squadQuery, IMemberQuery memberQuery, 
                ILibraryQuery libraryQuery, IAssignmentService assignmentService,
                IAssignmentQuery assignmentQuery
        )
        {
            this.club = club;
            this.squadQuery = squadQuery;
            this.memberQuery = memberQuery;
            this.libraryQuery = libraryQuery;
			this.assignmentService = assignmentService;
            this.assignmentQuery = assignmentQuery;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
			var model = new IndexViewModel();
			model.Squads = GetSquadList().OrderBy(s => s.Text).Prepend(new SelectListItem { Text = "All", Value = "" }).ToList();
			return View(model);
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
            if (!ModelState.IsValid)            
                return PromptForSubmission(model);
			else if(model.AssignedTo == AssignedTo.IndividualPlayers && (model.Players == null || model.Players.Count() == 0)){
				ModelState.AddModelError("", "Please specify which players this is assigned to");
				return PromptForSubmission(model);
			} else if (model.AssignedTo == AssignedTo.SelectedSquads && (model.Squads == null || model.Squads.Count() == 0)) {
				ModelState.AddModelError("", "Please specify which squads this is assigned to");
				return PromptForSubmission(model);
			}
            
            var email = User.Identity.Name;
            var members = memberQuery.GetMembersByEmail(club.Guid, email);
            var coach = members?.FirstOrDefault(m => m.Membership == Membership.Coach);

            if(coach == null)
            {
                ModelState.AddModelError("", "Coach could not be resolved");
                PromptForSubmission(model);
            }

            var response = assignmentService.CreateAssignment(new AssignmentRequest { 
                Title = model.Title,
				ClubId = club.Guid,
				CoachId = coach.Guid,
				DateDue = model.DateDue,
				Instructions = model.Instructions,
				Players = model.AssignedTo == AssignedTo.IndividualPlayers ? model.Players : null,
				Squads = model.AssignedTo == AssignedTo.SelectedSquads ? model.Squads : null,
				TrainingMaterials = model.TrainingMaterials,
				AssignedTo = model.AssignedTo.Value
			});

			if(!response.RequestIsFulfilled){				
				foreach (var error in response.Errors)
					ModelState.AddModelError("", error);

                PromptForSubmission(model);
				return View("Create", model);
			}

            return RedirectToAction(nameof(Index));
        }

        private IActionResult PromptForSubmission(AssignmentDetailsViewModel model)
        {
            model.SquadList = GetSquadList();
            model.SelectedPlayerList = GetSelectedPlayerList(model.Players);
            model.SelectedTrainingMaterialList = GetSelectedTrainingMaterialList(model.TrainingMaterials);

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