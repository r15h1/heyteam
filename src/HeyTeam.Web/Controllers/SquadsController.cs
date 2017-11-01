using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Entities;
using System.Collections.Generic;

namespace HeyTeam.Web.Controllers {
    
    [Authorize]
    [Route("[controller]")]
    public class SquadsController : Controller {
        private readonly Club club;
        private readonly IUseCase<AddSquadRequest, Response<Guid?>> addSquadUseCase;
        private readonly IUseCase<GetSquadRequest, Response<(Squad, IEnumerable<Player>)>> getSquadUseCase;

        public SquadsController(
                Club club, 
                IUseCase<AddSquadRequest, Response<Guid?>> addSquadUseCase,
                IUseCase<GetSquadRequest, Response<System.ValueTuple<Core.Entities.Squad, IEnumerable<Core.Entities.Player>>>> getSquadUseCase        
        ) {
            this.club = club;
            this.addSquadUseCase = addSquadUseCase;
            this.getSquadUseCase = getSquadUseCase;
        }

        [HttpGet("{squadId:guid}")]
        public IActionResult Details([FromRoute]string squadId, [FromQuery]string returnUrl) {
            ViewData["Title"] = "Squad Details";
            var request = new GetSquadRequest { ClubId = club.Guid, SquadId = System.Guid.Parse(squadId) };
            var response = getSquadUseCase.Execute(request);
            var model = new SquadDetailsViewModel { SquadName = response.Result.Item1.Name, SquadId = response.Result.Item1.Guid.ToString(), Players = response.Result.Item2 };
            return View("Details", model);
        }

        [HttpGet("new")]
        public IActionResult New(string returnUrl) {
            ViewData["Title"] = "Add New Squad";
            ViewData["ReturnUrl"] = returnUrl ?? "/";
            return View("Edit");
        }

        [HttpPost("new")]
        public IActionResult New([FromForm]SquadViewModel squad, [FromQuery]string returnUrl) {
            ViewData["Title"] = "Add New Squad";
            ViewData["ReturnUrl"] = returnUrl ?? "/";

            if (!ModelState.IsValid)
                return View("Edit", squad);
            

            var result = addSquadUseCase.Execute(new AddSquadRequest{
                SquadName = squad.SquadName,
                ClubId = club.Guid
            });
            
            if(result.WasRequestFulfilled) {
                returnUrl = returnUrl ?? "/";
                return RedirectToLocal(returnUrl);
            } else {
                foreach(var error in result.Errors)
                    ModelState.AddModelError("", error);                
                
                return View("Edit", squad);
            }            
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}