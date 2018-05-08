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
    public class AreaSelectionController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly Club club;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMemberQuery memberQuery;
		private readonly SignInManager<ApplicationUser> signInManager;

		public AreaSelectionController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery, SignInManager<ApplicationUser> signInManager) 
        {
            this.club = club;
            this.userManager = userManager;
            this.memberQuery = memberQuery;
			this.signInManager = signInManager;
		}

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
            

            var members = memberQuery.GetMembersByEmail(club.Guid, user.Email);

			if (!members.Any()) {
				ModelState.AddModelError("", "Invalid login");
				await signInManager.SignOutAsync();
				return RedirectToAction("Login", "Accounts");
			} else if (isAdmin) {
				return RedirectToAction("Index", "Home", new { Area = "Administration" });
			} else if (members.Count() > 1) {
				return View(members);
			}

			if (members.Any(m => m.Membership == Membership.Coach)) {
				bool isCoach = await userManager.IsInRoleAsync(user, "Coach");
				if (isCoach)
					return RedirectToAction("Index", "Home", new { Area = "Coaches" });
			}

			return RedirectToAction("Index", "Home", new { Area = "Players", memberid = members.FirstOrDefault().Guid});            
        }
    }
}