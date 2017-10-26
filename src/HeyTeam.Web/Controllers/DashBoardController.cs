using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.UseCases;
using HeyTeam.Identity;
using HeyTeam.Web.Models.DashboardViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers {

    [Authorize]
    public class DashboardController : Controller {
        private readonly IUseCase<DashboardRequest, Response<List<Group>>> useCase;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(IUseCase<DashboardRequest, Response<List<Group>>> useCase, UserManager<ApplicationUser> userManager) {
            this.useCase = useCase;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() {
            var user = userManager.GetUserAsync(User).Result;
            var request = new DashboardRequest {
                Email = user.Email,
                ClubId = System.Guid.Parse("b58795e7-99f8-4b0a-8292-a05ed533556c")
            };
            var response = useCase.Execute(request);
            IndexViewModel viewModel = CreateViewModel(response.Result);
            return View(viewModel);
        }

        private IndexViewModel CreateViewModel(List<Group> result)
        {
            IndexViewModel model = new IndexViewModel();
            if(result.Any(r => r.Name.ToLowerInvariant().Equals("squads")))
                model.Squads = result.FirstOrDefault(r => r.Name.ToLowerInvariant().Equals("squads")).Items;

            return model;
        }
    }
}