using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.UseCases;
using HeyTeam.Core.Entities;

namespace HeyTeam.Web.Controllers {
    
    [Authorize]
    [Route("[controller]")]
    public class SquadsController : Controller {
        private readonly Club club;
        private readonly IUseCase<AddSquadRequest, Response<Guid?>> addUseCase;

        public SquadsController(Club club, IUseCase<AddSquadRequest, Response<Guid?>> addUseCase) {
            this.club = club;
            this.addUseCase = addUseCase;
        }

        [HttpGet("{squadId}")]
        public IActionResult Details([FromRoute]string squadId, [FromQuery]string returnUrl) {
            ViewData["Title"] = "Add New Squad";
            ViewData["ReturnUrl"] = returnUrl ?? "/";
            return View("Details");
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
            

            var result = addUseCase.Execute(new AddSquadRequest{
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