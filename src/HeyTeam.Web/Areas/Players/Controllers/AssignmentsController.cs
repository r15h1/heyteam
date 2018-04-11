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

namespace HeyTeam.Web.Areas.Players.Controllers
{
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}/[controller]")]
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
        public IActionResult Index(Guid memberid)
        {
			var model = new IndexViewModel();
			model.PlayerId = memberid;
			return View(model);
        }        
    }
}