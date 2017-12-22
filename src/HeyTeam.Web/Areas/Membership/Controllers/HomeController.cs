using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Areas.Membership.Controllers {
	[Authorize(Policy = "PlayerOrCoach")]
	[Area("Membership")]
	[Route("[area]/{memberid:guid}")]
	[Route("[area]/{memberid:guid}/[controller]")]
	public class HomeController : Controller
    {
        private readonly Club club;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMemberQuery memberQuery;

        public HomeController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) 
        {
            this.club = club;
            this.userManager = userManager;
            this.memberQuery = memberQuery;
        }        
               
        [HttpGet]
        public IActionResult Index(Guid memberid)
        {
            var user = userManager.GetUserAsync(User).Result;
            var request = new DashboardRequest
            {
                UserEmail = user.Email,
                ClubId = club.Guid
            };
            
            return View("Index");
        }
    }
}