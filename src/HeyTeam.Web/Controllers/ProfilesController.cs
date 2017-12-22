using HeyTeam.Core;
using HeyTeam.Core.Queries;
using HeyTeam.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HeyTeam.Web.Controllers
{
    [Authorize]    
    public class ProfilesController : Controller
    {
        private readonly Club club;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMemberQuery memberQuery;

        public ProfilesController(Club club, UserManager<ApplicationUser> userManager, IMemberQuery memberQuery)
        {
            this.club = club;
            this.userManager = userManager;
            this.memberQuery = memberQuery;
        }

        [HttpGet("")]
        public IActionResult ProfileSelection()
        {
            var user = userManager.GetUserAsync(User).Result;
            bool isAdmin = userManager.IsInRoleAsync(user, "Administrator").Result;
            if(isAdmin)
                return RedirectToAction("Index", "Home", new { Area = "Administration" });

            var members = memberQuery.GetMembersByEmail(club.Guid, user.Email);            
            if (members.Count() > 1)
                return View("ProfileSelector", members);
            
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