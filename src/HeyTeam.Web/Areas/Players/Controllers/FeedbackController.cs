using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeyTeam.Web.Areas.Players.Controllers
{
	[Authorize(Policy = "Player")]
	[Area("Players")]
	[Route("[area]/{memberid:guid}")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class FeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}