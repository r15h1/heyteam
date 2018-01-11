using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Controllers
{
    [Authorize]     
    [Route("")]
    public class AreaSelectionController : Controller
    {
        private readonly Club club;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMemberQuery memberQuery;

        public AreaSelectionController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) 
        {
            this.club = club;
            this.userManager = userManager;
            this.memberQuery = memberQuery;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
            if(isAdmin)
                return RedirectToAction("Index", "Home", new { Area = "Administration" });

            var members = memberQuery.GetMembersByEmail(club.Guid, user.Email);            
            if (members.Count() > 1)
                return View(members);

			bool isCoach = await userManager.IsInRoleAsync(user, "Coach");
			if (isCoach)
				return RedirectToAction("Index", "Home", new { Area = "Coaching" });

			return RedirectToAction("Index", "Home", new { Area = "Membership", memberid = members.FirstOrDefault().Guid});            
        }
    }
}