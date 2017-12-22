using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Membership.Controllers
{
    [Authorize] 
    [Area("Membership")]
    [Route("[area]/[controller]")]
    public class MembersController : Controller
    {
        private readonly Club club;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMemberQuery memberQuery;

        public MembersController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery) 
        {
            this.club = club;
            this.userManager = userManager;
            this.memberQuery = memberQuery;
        }

        [HttpGet("")]
        public async Task<IActionResult> MemberSelection()
        {
            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
            if(isAdmin)
                return RedirectToAction("Index", "Home", new { Area = "Administration" });

            var members = memberQuery.GetMembersByEmail(club.Guid, user.Email);            
            if (members.Count() > 1)
                return View("MemberSelector", members);
            
            return RedirectToAction("Index", new {profileid = members.FirstOrDefault().Guid});            
        }

        [Authorize(Policy = "PlayerOrCoach")]        
        [HttpGet("[controller]/{profileid:guid}")]
        public IActionResult Index(Guid profileid)
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