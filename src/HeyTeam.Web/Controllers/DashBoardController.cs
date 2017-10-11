using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Controllers {

    [Authorize]
    public class DashBoardController : Controller {

        [HttpGet]
        public IActionResult Index() {
            return View();
        }
    }
}