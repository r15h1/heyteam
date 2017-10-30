using Microsoft.AspNetCore.Mvc;
using System;
using HeyTeam.Web.Models.SquadViewModels;
using Microsoft.AspNetCore.Authorization;
using HeyTeam.Core.UseCases.Squad;
using HeyTeam.Core.UseCases;

namespace HeyTeam.Web.Controllers {
    
    [Authorize]
    [Route("[controller]/[action]")]
    public class SquadsController : Controller {
        private readonly IUseCase<AddSquadRequest, Response<Guid?>> addUseCase;

        public SquadsController(IUseCase<AddSquadRequest, Response<Guid?>> addUseCase) {
            this.addUseCase = addUseCase;
        }

        [HttpGet]
        public IActionResult New(string returnUrl) {
            ViewData["Title"] = "Add New Squad";
            ViewData["ReturnUrl"] = returnUrl ?? "/";
            return View("SquadDetail");
        }

        [HttpPost]
        public IActionResult New([FromForm]SquadViewModel squad, [FromQuery]string returnUrl) {
            if (!ModelState.IsValid) 
                return View(squad);

            var result = addUseCase.Execute(new AddSquadRequest{
                SquadName = squad.SquadName,
                ClubId = System.Guid.Parse("b58795e7-99f8-4b0a-8292-a05ed533556c")
            });
            
            if(result.WasRequestFulfilled) {
                returnUrl = returnUrl ?? "/";
                return RedirectToLocal(returnUrl);
            } else {
                foreach(var error in result.Errors)
                    ModelState.AddModelError("", error);
                
                ViewData["Title"] = "Add New Squad";
                ViewData["ReturnUrl"] = returnUrl ?? "/";
                return View("SquadDetail", squad);
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