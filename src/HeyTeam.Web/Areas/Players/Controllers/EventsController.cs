using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeyTeam.Web.Areas.Players.Controllers
{
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}