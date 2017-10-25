using System.Collections.Generic;
using System.Threading.Tasks;
using HeyTeam.Core.Dashboard;
using HeyTeam.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers {

    [Authorize]
    public class DashboardController : Controller {
        private readonly IUseCase<DashboardRequest, Response<List<Group>>> useCase;

        public DashboardController(IUseCase<DashboardRequest, Response<List<Group>>> useCase) {
            this.useCase = useCase;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }
    }
}