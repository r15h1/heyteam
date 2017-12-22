using HeyTeam.Core;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using HeyTeam.Web.Models.DashboardViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeyTeam.Web.Areas.Administration.Controllers
{

    [Authorize(Policy = "Administrator")]
    [Area("Administration")]
    [Route("[area]/[controller]")]
    public class HomeController : Controller {
        private readonly Club club;
        private readonly IDashboardQuery dashboardQuery;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(Club club, IDashboardQuery dashboardQuery, UserManager<ApplicationUser> userManager) {
            this.club = club;
            this.dashboardQuery = dashboardQuery;
            this.userManager = userManager;
        }

        
        [HttpGet]
        public IActionResult Index() {
            var user = userManager.GetUserAsync(User).Result;
            var request = new DashboardRequest {
                UserEmail = user.Email,
                ClubId = club.Guid
            };
            var response = dashboardQuery.GetDashboard(request);
            if (response.Errors != null && response.Errors.Any()) {
                return View(new IndexViewModel { Errors = response.Errors });
            }

            IndexViewModel viewModel = CreateViewModel(response.Dashboard);
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